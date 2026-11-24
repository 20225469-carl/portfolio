INSERT INTO portfolio.course (code, title, term, description) VALUES
('DBMWEB1', 'Database Management and Web Systems', '1st Sem SY 2026-2027', 'Full-stack ASP.NET Core MVC applications backed by PostgreSQL.'),
('APPDEV2', 'Mobile and Cross-Platform Development', '2nd Sem SY 2025-2026', 'Designing responsive interfaces and RESTful API integrations.');

INSERT INTO portfolio.project (course_id, title, summary, video_url, repo_url, year_created) VALUES
(1, 'Library Management System', 'A full-featured library system with ACID transactions, regex validation, and error handling.', 'https://www.youtube.com/watch?v=dQw4w9WgXcQ', 'https://github.com/student/library', 2026),
(1, 'Student Academic Portfolio', 'A dual-mode viewer/editor web portfolio showcasing course projects and media.', 'https://youtu.be/dQw4w9WgXcQ', 'https://github.com/student/portfolio', 2026);

INSERT INTO portfolio.project_image (project_id, image_url) VALUES
(1, 'https://placehold.co/800x450/111111/FFFFFF?text=Library+Loans+Dashboard'),
(1, 'https://placehold.co/800x450/222222/FFFFFF?text=Member+Borrow+History'),
(2, 'https://placehold.co/800x450/000000/FFFFFF?text=Portfolio+Viewer+Screen');