CREATE SCHEMA IF NOT EXISTS portfolio;

DROP TABLE IF EXISTS portfolio.project_image CASCADE;
DROP TABLE IF EXISTS portfolio.project CASCADE;
DROP TABLE IF EXISTS portfolio.course CASCADE;

CREATE TABLE portfolio.course (
    course_id   BIGINT GENERATED ALWAYS AS IDENTITY,
    code        VARCHAR(15)  NOT NULL,
    title       VARCHAR(120) NOT NULL,
    term        VARCHAR(30)  NOT NULL,
    description TEXT,
    CONSTRAINT pk_course PRIMARY KEY (course_id),
    CONSTRAINT uq_course_code UNIQUE (code),
    CONSTRAINT ck_course_code_format CHECK (code ~ '^[A-Z]{2,6}[0-9]{1,4}$')
);

CREATE TABLE portfolio.project (
    project_id   BIGINT GENERATED ALWAYS AS IDENTITY,
    course_id    BIGINT       NOT NULL,
    title        VARCHAR(150) NOT NULL,
    summary      TEXT         NOT NULL,
    video_url    VARCHAR(255),
    repo_url     VARCHAR(255),
    year_created INT          NOT NULL,
    CONSTRAINT pk_project PRIMARY KEY (project_id),
    CONSTRAINT fk_project_course FOREIGN KEY (course_id)
        REFERENCES portfolio.course (course_id) ON DELETE RESTRICT,
    CONSTRAINT uq_project_title_per_course UNIQUE (course_id, title),
    CONSTRAINT ck_project_year CHECK (year_created BETWEEN 2020 AND 2030)
);

CREATE TABLE portfolio.project_image (
    image_id   BIGINT GENERATED ALWAYS AS IDENTITY,
    project_id BIGINT       NOT NULL,
    image_url  VARCHAR(500) NOT NULL,
    CONSTRAINT pk_project_image PRIMARY KEY (image_id),
    CONSTRAINT fk_image_project FOREIGN KEY (project_id)
        REFERENCES portfolio.project (project_id) ON DELETE CASCADE,
    CONSTRAINT uq_project_image_url UNIQUE (project_id, image_url),
    CONSTRAINT ck_image_url_https CHECK (image_url ~ '^https?://')
);