-- TABLAS
CREATE TABLE "Person" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(30) NOT NULL,
    "LastName" VARCHAR(30) NOT NULL,
    "Email" VARCHAR(100) NOT NULL,
    "DocumentNumber" VARCHAR(10) NOT NULL,
    "Phone" VARCHAR(15) NOT NULL,
    "Address" VARCHAR(100) NOT NULL,
    "DocumentType" CHAR(3) NOT NULL,
    "BloodType" CHAR(3) NOT NULL,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE,
    CHECK ("DocumentNumber" ~ '^[0-9]+$'),
    CHECK ("DocumentType" IN ('RC', 'TI', 'CC', 'CE', 'NIT', 'PP')),
    CHECK ("BloodType" IN ('A+', 'A-', 'B+', 'B-', 'AB+', 'AB-', 'O+', 'O-'))
);

CREATE TABLE "User" (
    "Id" SERIAL PRIMARY KEY,
    "Username" VARCHAR(50) NOT NULL,
    "Password" VARCHAR(100) NOT NULL,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE,
    "PersonId" INT NOT NULL
);

CREATE TABLE "Rol" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(200),
    "Active" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE "Permission" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(200) NOT NULL,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE "Form" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(200) NOT NULL,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE "Module" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(50) NOT NULL,
    "Description" VARCHAR(200),
    "Active" BOOLEAN NOT NULL DEFAULT TRUE
);

CREATE TABLE "RolUser" (
    "Id" SERIAL PRIMARY KEY,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE,
    "RolId" INT NOT NULL,
    "UserId" INT NOT NULL
);

CREATE TABLE "RolFormPermission" (
    "Id" SERIAL PRIMARY KEY,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE,
    "RolId" INT NOT NULL,
    "FormId" INT NOT NULL,
    "PermissionId" INT NOT NULL
);

CREATE TABLE "FormModule" (
    "Id" SERIAL PRIMARY KEY,
    "Active" BOOLEAN NOT NULL DEFAULT TRUE,
    "FormId" INT NOT NULL,
    "ModuleId" INT NOT NULL
);

-- RELACIONES
ALTER TABLE "User"
ADD CONSTRAINT "FK_User_Person" FOREIGN KEY ("PersonId") REFERENCES "Person"("Id") ON DELETE CASCADE;

ALTER TABLE "RolUser"
ADD CONSTRAINT "FK_RolUser_Rol" FOREIGN KEY ("RolId") REFERENCES "Rol"("Id") ON DELETE CASCADE,
ADD CONSTRAINT "FK_RolUser_User" FOREIGN KEY ("UserId") REFERENCES "User"("Id") ON DELETE CASCADE;

ALTER TABLE "RolFormPermission"
ADD CONSTRAINT "FK_RolFormPermission_Rol" FOREIGN KEY ("RolId") REFERENCES "Rol"("Id") ON DELETE CASCADE,
ADD CONSTRAINT "FK_RolFormPermission_Form" FOREIGN KEY ("FormId") REFERENCES "Form"("Id") ON DELETE CASCADE,
ADD CONSTRAINT "FK_RolFormPermission_Permission" FOREIGN KEY ("PermissionId") REFERENCES "Permission"("Id") ON DELETE CASCADE;

ALTER TABLE "FormModule"
ADD CONSTRAINT "FK_FormModule_Form" FOREIGN KEY ("FormId") REFERENCES "Form"("Id") ON DELETE CASCADE,
ADD CONSTRAINT "FK_FormModule_Module" FOREIGN KEY ("ModuleId") REFERENCES "Module"("Id") ON DELETE CASCADE;
