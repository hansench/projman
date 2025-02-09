CREATE TABLE "AppUser"
(
    "Id" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UserName" VARCHAR(200) UNIQUE NOT NULL,
    "FullName" VARCHAR(200) NOT NULL,
    "RoleId" INT NOT NULL,
    "Hashed" BYTEA NULL,
    "Salted" BYTEA NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT(TRUE),
	"InsertedUserId" INT NOT NULL,
	"InsertedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
	"UpdatedUserId" INT NOT NULL DEFAULT(0),
	"UpdatedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP)
);

CREATE TABLE "AppUserRefrehToken"
(
    "Id" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Token" TEXT NOT NULL,
    "InsertedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
    "ExpiredUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "IsActive" BOOLEAN NOT NULL DEFAULT(TRUE)
)


CREATE TABLE "ProjectStage"
(
    "Id" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "Name" VARCHAR(200) UNIQUE NOT NULL,
    "Descr" VARCHAR(200) NOT NULL,
	"InsertedUserId" INT NOT NULL,
	"InsertedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
	"UpdatedUserId" INT NOT NULL DEFAULT(0),
	"UpdatedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP)
);


CREATE TABLE "ProjectCategory"
(
    "Id" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "Name" VARCHAR(200) UNIQUE NOT NULL,
    "Descr" VARCHAR(200) NOT NULL,
	"InsertedUserId" INT NOT NULL,
	"InsertedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
	"UpdatedUserId" INT NOT NULL DEFAULT(0),
	"UpdatedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP)
);


CREATE TABLE "Project"
(
	"Id" INT GENERATED ALWAYS AS IDENTITY (START WITH 100000 INCREMENT 1) PRIMARY KEY,
	"ProjectName" VARCHAR(200) NOT NULL,
	"ProjectLocation" VARCHAR(500) NOT NULL,
	"ProjectDetails" VARCHAR(2000) NOT NULL,
	"StageId" INT NOT NULL,
	"CategoryId" INT NOT NULL,
	"CategoryOthersDescr" VARCHAR(200) NOT NULL,
	"StartDate" DATE NOT NULL,
	"InsertedUserId" INT NOT NULL,
	"InsertedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
	"UpdatedUserId" INT NOT NULL DEFAULT(0),
	"UpdatedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
    CONSTRAINT fk_project_stage FOREIGN KEY("StageId") REFERENCES "ProjectStage"("Id"),
    CONSTRAINT fk_project_category FOREIGN KEY("CategoryId") REFERENCES "ProjectCategory"("Id")
);

CREATE TABLE "ProjectStageDetail"
(
    "Id" INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    "ProjectId" INT NOT NULL,
    "StageId" INT NOT NULL,
    "Remarks" TEXT NOT NULL,
    "InsertedUserId" INT NOT NULL,
	"InsertedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
	"UpdatedUserId" INT NOT NULL DEFAULT(0),
	"UpdatedUtc" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT (NOW()::TIMESTAMP),
    CONSTRAINT fk_project_stage_project FOREIGN KEY("ProjectId") REFERENCES "Project"("Id") ON DELETE CASCADE,
    CONSTRAINT fk_project_stage_stage FOREIGN KEY("StageId") REFERENCES "ProjectStage"("Id")
);


INSERT INTO "ProjectStage" ("Name", "Descr", "InsertedUserId") VALUES ('Concept', 'Stage Concept', 0);
INSERT INTO "ProjectStage" ("Name", "Descr", "InsertedUserId") VALUES ('Design and Documentation', 'Stage Design and Documentation', 0);
INSERT INTO "ProjectStage" ("Name", "Descr", "InsertedUserId") VALUES ('Pre-Construction', 'Stage Pre-Construction', 0);
INSERT INTO "ProjectStage" ("Name", "Descr", "InsertedUserId") VALUES ('Construction', 'Stage Construction', 0);

INSERT INTO "ProjectCategory" ("Name", "Descr", "InsertedUserId") VALUES ('Education', 'Category Education', 0);
INSERT INTO "ProjectCategory" ("Name", "Descr", "InsertedUserId") VALUES ('Health', 'Category Health', 0);
INSERT INTO "ProjectCategory" ("Name", "Descr", "InsertedUserId") VALUES ('Office', 'Category Office', 0);
INSERT INTO "ProjectCategory" ("Name", "Descr", "InsertedUserId") VALUES ('Others', 'Category Others', 0);


