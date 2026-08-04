# GroceryMart: End-to-End CI/CD Pipeline & Blue-Green Deployment Architecture

[![Build & Deploy Pipeline](https://img.shields.io/badge/Azure%20DevOps-Pipelines-blue?logo=azuredevops)](https://dev.azure.com/)
[![IaC](https://img.shields.io/badge/IaC-Terraform_v1.15.8-purple?logo=terraform)](https://www.terraform.io/)
[![Framework](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Cloud](https://img.shields.io/badge/Azure-App_Services-0078D4?logo=microsoftazure)](https://portal.azure.com/)

An enterprise-grade, fully automated CI/CD pipeline and Infrastructure as Code (IaC) implementation for **GroceryMart**—a high-availability .NET 8 web application. This project demonstrates zero-downtime releases, automated quality gates, and instant rollback capabilities on Microsoft Azure using a Blue-Green deployment strategy.

---

## 📋 Table of Contents
- [Executive Summary & Objectives](#-executive-summary--objectives)
- [Architecture & Workflow](#-architecture--workflow)
- [Project Directory Structure](#-project-directory-structure)
- [Tech Stack & Prerequisites](#-tech-stack--prerequisites)
- [Infrastructure as Code (Terraform)](#-infrastructure-as-code-terraform)
- [CI/CD Pipeline Details](#-cicd-pipeline-details)
- [Blue-Green Deployment & Zero-Downtime Releases](#-blue-green-deployment--zero-downtime-releases)
- [Local Development & Setup](#-local-development--setup)
- [Verification & Observability](#-verification--observability)
- [Conclusion & Best Practices](#-conclusion--best-practices)

---

## 🎯 Executive Summary & Objectives

The primary objective of this project is to establish an end-to-end continuous integration and continuous deployment strategy for enterprise web applications on Azure.

### Key Deliverables & Outcomes:
* **Infrastructure as Code (IaC):** Automated provisioning of Azure resources (App Service, Deployment Slots, VNet, NSG, and Self-Hosted Linux Build Agent) using Terraform.
* **Automated CI/CD:** Multi-stage YAML pipelines executing automated builds, unit testing, artifact packaging, and environment deployments.
* **Zero-Downtime Production Releases:** Implementation of Blue-Green deployment slots on Azure App Service.
* **Instant Rollback & Resilience:** Warm fallback retention post-swap to reduce Mean Time to Recovery (MTTR) to seconds in case of production regression.
* **Production Quality Gates:** Automated smoke/health checks paired with manual environment approvals.

---

## 🏗️ Architecture & Workflow

```
+---------------------------------------------------------------------------------------------------+
|                                      AZURE DEVOPS ENVIRONMENT                                     |
|                                                                                                   |
|  +------------------------+      +---------------------------------+      +--------------------+  |
|  |       AZURE REPOS      | ---> |       CI PIPELINE (BUILD)       | ---> |  BUILD ARTIFACTS   |  |
|  |  (Source & Policies)   |      | Self-Hosted Agent / Unit Tests  |      |     (.zip)         |  |
|  +------------------------+      +---------------------------------+      +---------+----------+  |
+-------------------------------------------------------------------------------------|-------------+
                                                                                      |
+-------------------------------------------------------------------------------------v-------------+
|                                    CD PIPELINE & AZURE RUNTIME                                    |
|                                                                                                   |
|  Stage 1: Development    --->    Stage 2: Staging    --->    Stage 3: Production (Blue-Green)    |
|  (Dev App Service)               (Staging App Service)         +-------------------------------+  |
|                                                                | Deploy to Green (Staging) Slot|  |
|                                                                | Automated Health Checks       |  |
|                                                                | Manual Gate Approval          |  |
|                                                                | Swap Slot (Green -> Blue)     |  |
|                                                                +---------------+---------------+  |
|                                                                                |                  |
|                                                                                v                  |
|                                                         +--------------------------------------+  |
|                                                         |          AZURE APP SERVICE           |  |
|                                                         |  [BLUE SLOT (Live)] <--> [GREEN SLOT]|  |
|                                                         +--------------------------------------+  |
+---------------------------------------------------------------------------------------------------+
```

1. **Developer Workflow:** Code commits pushed to Azure Repos trigger the CI pipeline.
2. **Continuous Integration (CI):** Self-hosted Linux agent restores dependencies (`dotnet restore`), builds solution (`dotnet build`), executes unit tests (`dotnet test`), and publishes `.zip` build artifacts.
3. **Continuous Deployment (CD):** Multi-stage release progression through Development, Staging, and Production.
4. **Blue-Green Deployment:** Production updates are deployed to the **Green Slot**, validated via health endpoints, approved through pipeline gates, and swapped live to the **Blue Slot** without dropping requests.

---

## 📂 Project Directory Structure

```
grocery-mart-app/
├── .azuredevops/
│   └── pipelines/
│       ├── azure-pipelines-ci.yml       # Continuous Integration Pipeline
│       └── azure-pipelines-cd.yml       # Multi-Stage Continuous Deployment Pipeline
├── terraform/
│   ├── main.tf                          # Primary IaC resource definitions
│   ├── variables.tf                     # Input variable definitions
│   ├── outputs.tf                       # Infrastructure output values
│   ├── provider.tf                      # AzureRM provider configuration
│   └── terraform.tfvars                 # Environment variable overrides
├── src/
│   ├── GroceryMart.sln                  # Visual Studio Solution
│   ├── GroceryMart/                     # ASP.NET Core 8.0 Web Application
│   │   ├── Controllers/                 # MVC Controllers (Auth, Health, Store, Home)
│   │   ├── Data/                        # Entity Framework Core AppDbContext
│   │   ├── Migrations/                  # EF Database Migrations
│   │   ├── Models/                      # Application & Domain Models
│   │   ├── Views/                       # Razor Views & Layouts
│   │   ├── wwwroot/                     # Static assets (CSS, JS, Images)
│   │   ├── appsettings.json             # Environment Configuration
│   │   └── Program.cs                   # Application Entry Point & Middleware
│   └── GroceryMart.Tests/               # Unit & Integration Test Suites
├── .gitignore
└── README.md                            # Project Documentation
```

---

## 🛠️ Tech Stack & Prerequisites

### Core Frameworks & Tools
* **Application Framework:** .NET 8.0 (ASP.NET Core MVC)
* **Database:** Entity Framework Core (SQLite / Azure SQL Database)
* **Cloud Infrastructure:** Microsoft Azure (App Service, Deployment Slots, VNet, NSG, VM)
* **Infrastructure as Code:** Terraform v1.15.8+ (AzureRM Provider ~> 3.117.1)
* **CI/CD Orchestration:** Azure DevOps Pipelines (YAML Multi-Stage)
* **Build Agent:** Self-Hosted Linux Virtual Machine (`Ubuntu 22.04 LTS`)

---

## 🧱 Infrastructure as Code (Terraform)

Infrastructure is managed modularly using HashiCorp Terraform.

### Provisioned Resources (`rg-grocerymart-prod`)
* **Resource Group:** `rg-grocerymart-prod`
* **App Service Plan:** `asp-grocerymart-prod`
* **App Service (Production/Blue):** `app-grocerymart-prod`
* **Deployment Slot (Green):** `app-grocerymart-prod-green`
* **Virtual Network & Subnet:** `vnet-grocerymart-prod` (`10.0.0.0/16`)
* **Build Agent Infrastructure:**
  * Virtual Machine: `vm-agent-grocerymart`
  * Network Interface: `nic-agent-grocerymart`
  * Network Security Group: `nsg-agent-grocerymart`
  * Public IP: `pip-agent-grocerymart`

### Provisioning Execution Workflow

```bash
# Initialize Terraform and download provider plugins
cd terraform
terraform init

# Validate syntax and configuration integrity
terraform validate

# Plan infrastructure changes
terraform plan -out=tfplan.binary

# Apply infrastructure deployment
terraform apply -auto-approve tfplan.binary
```

---

## 🚀 CI/CD Pipeline Details

### 1. Build Stage (`azure-pipelines-ci.yml`)
* **Agent:** Self-hosted Linux VM (`vm-agent-grocerymart`)
* **Tasks:**
  1. SDK Setup: `.NET 8.0 SDK` installation check.
  2. Restore: `dotnet restore` execution for dependencies.
  3. Build: `dotnet build --configuration Release`.
  4. Test: `dotnet test` quality gate verification.
  5. Publish: `dotnet publish` output package creation.
  6. Artifact: Publish zipped artifact to Azure DevOps Artifact Store.

### 2. Deployment Stages (`azure-pipelines-cd.yml`)
* **Development & Staging:** Deploy artifact to Dev/Staging App Services for automated health checks.
* **Production Blue-Green:**
  * **Step 1:** Deploy release package to `Green` (Staging) slot (`app-grocerymart-prod-green`).
  * **Step 2:** Execute automated smoke tests (`/health` endpoint validation).
  * **Step 3:** Trigger manual gate approval for release engineering teams.
  * **Step 4:** Execute Azure App Service **Slot Swap** (`Green` $ightarrow$ `Blue`).

---

## 🔄 Blue-Green Deployment & Zero-Downtime Releases

### Strategy Overview
1. **Live Traffic:** Requests route directly to the **Blue Slot** (`v1.0`).
2. **Deployment:** The pipeline deploys new code (`v2.0`) into the staging **Green Slot** without affecting active users.
3. **Validation:** Health endpoints and smoke tests confirm `v2.0` operational status on Green.
4. **Slot Swap:** Azure App Service rewrites routing rules instantly. Live traffic flows to `v2.0`.
5. **Rollback Assurance:** `v1.0` is kept warm in the Green slot. If any post-release issues arise, a reverse swap restores `v1.0` in seconds.

---

## 💻 Local Development & Setup

### Prerequisites
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Git](https://git-scm.com/)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
* [Terraform CLI](https://developer.hashicorp.com/terraform/downloads)

### Local Execution Steps
```bash
# Clone Repository
git clone https://github.com/praveenbhise07-art/grocerymart-webapp.git
cd grocerymart-webapp/src/GroceryMart

# Apply Entity Framework Core Migrations
dotnet ef database update

# Run Application Locally
dotnet run
```
Access application at `http://localhost:5010`.

---

## 🧪 Verification & Observability

* **Application Health Endpoint:** `https://app-grocerymart-prod.azurewebsites.net/health`
* **Green Slot Verification:** `https://app-grocerymart-prod-green.azurewebsites.net/`
* **Telemetry & Logging:** Integrated with Azure Application Insights and Azure Log Analytics Workspace for real-time error tracking and latency metrics.

---

## 🏁 Conclusion & Best Practices

This architecture successfully satisfies production requirements for enterprise applications:
* **Zero Downtime:** Seamless traffic migration using native App Service slots.
* **Resilience:** Immediate MTTR reduction via warm fallback slots.
* **Governance:** Modular Terraform IaC ensures reproducible infrastructure with zero drift.
* **Quality Assurance:** Mandatory build success, unit test coverage, and gate approvals prior to production exposure.
