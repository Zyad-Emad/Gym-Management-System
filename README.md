# Gym Management System

A web-based gym management platform built with **ASP.NET Core MVC** to demonstrate clean layered architecture, EF Core data access, and real-world business rule handling.

## Overview

This project simulates a real gym's day-to-day operations — member registration, trainer scheduling, session booking, and membership plans. I built it to practice a proper **three-layer architecture** (Presentation / Business Logic / Data Access) the way it's actually done in production .NET applications, rather than putting everything in the controllers.

The system handles multiple related entities (members, trainers, sessions, plans, bookings) with real constraints like capacity limits, active-membership checks, and date-based booking rules, all enforced through the business layer rather than the database alone.

## Features

**Core Functionality**
- Full CRUD for Members, Trainers, Sessions, and Plans
- Health record tracking, one per member
- Membership system linking Members to Plans with auto-calculated expiry
- Session booking system linking Members to Sessions with capacity and timing checks
- Soft-delete for Plans (Activate/Deactivate instead of removing them)
- Identity-based authentication and role access

**Advanced Features**
- Computed membership status (Active / Expired) based on plan duration, not stored directly
- Booking validation chain — active membership required, no double-booking, no booking past sessions
- Attendance tracking that's only allowed while a session is ongoing
- Egyptian phone number and email format validation at the entity configuration level
- Deletion guards — trainers/members with future sessions or active bookings can't be removed

## Technical Implementation

### Project Structure

```
GymManagementSolution/
├── GymManagement.PL/          # Presentation Layer
│   ├── Controllers/           # Member, Trainer, Session, Plan, MemberPlan, MemberSession, Account
│   ├── Views/                 # Razor views + Bootstrap UI
│   └── wwwroot/
├── GymManagement.BLL/         # Business Logic Layer
│   ├── Services/              # MemberService, TrainerService, SessionService, etc.
│   └── Interfaces/
└── GymManagement.DAL/         # Data Access Layer
    ├── Entities/               # Member, Trainer, Session, Plan, Booking, Membership...
    ├── Repositories/           # Repository pattern wrapping EF Core
    ├── Data/                   # DbContext + Migrations
    └── UnitOfWork/
```

### Architecture

**Three Layers:**
- **Presentation (PL)** — MVC Controllers + Razor Views, no business logic here
- **Business Logic (BLL)** — Services that own validation and business rules
- **Data Access (DAL)** — Repository + Unit of Work wrapping the EF Core DbContext

**Relationships:**
- Member → HealthRecord (1:1)
- Member ↔ Plan (via Membership, 1 active at a time)
- Member ↔ Session (via Booking, M:N)
- Trainer → Session (1:M)
- Category → Session (1:M)

**Patterns:**
- Repository Pattern + Unit of Work for data access
- Dependency Injection throughout
- AutoMapper for Entity ↔ ViewModel conversion

## Key Concepts Demonstrated

**Layered Architecture**
Each layer only talks to the one directly below it — controllers never touch the DbContext directly, they call services, which call repositories.

**Repository Pattern & Unit of Work**
Repositories abstract away EF Core so the business layer doesn't depend on it directly. The Unit of Work coordinates multiple repository operations under one transaction (e.g. creating a membership and updating a member in the same save).

**Business Rule Enforcement**
Rather than relying only on database constraints, rules like "a member can only have one active membership" or "a session can't be booked once it's started" live in the service layer, where they're easier to test and change.

**Entity Relationships**
Junction entities (`Booking`, `Membership`) carry their own data — `Attended`, `StartDate`/`EndDate` — instead of being plain many-to-many link tables.

## Design Decisions

**Why a separate Business Logic Layer?**
Controllers should only handle HTTP concerns — receiving requests and returning views. Putting validation and rules in services keeps that logic reusable and testable independent of MVC.

**Why Repository + Unit of Work instead of using DbContext directly?**
It decouples the business layer from EF Core specifics and makes it possible to swap out or mock the data layer later, plus it keeps multi-step operations (like membership creation + member update) atomic.

**Why compute Membership status instead of storing it?**
Status depends on the current date relative to `EndDate` — storing it would mean it goes stale. Computing it on read keeps it always accurate.

## What I Learned

This project helped me get hands-on with:
- How a layered architecture actually separates concerns in practice, not just in theory
- Designing entity relationships that need extra data on the junction (`Booking`, `Membership`) rather than plain many-to-many
- Writing business rules that depend on time (active vs expired, future vs past sessions) cleanly
- EF Core configuration — unique constraints, regex validation, decimal precision, computed-on-read values
- Structuring a solution across multiple projects (`.PL` / `.BLL` / `.DAL`) instead of one big project

The most challenging part was getting the booking and membership rules right — making sure capacity, timing, and active-membership checks all worked together without conflicting edge cases.

## Technologies Used

- **Language/Framework:** C# / ASP.NET Core MVC
- **Data Access:** Entity Framework Core, SQL Server
- **Mapping:** AutoMapper
- **Auth:** ASP.NET Core Identity
- **Frontend:** Razor Views, Bootstrap
- **Patterns:** Repository, Unit of Work, Dependency Injection

