-- ============================================
-- NOTES SYSTEM CUSTOMER DATABASE SETUP
-- MySQL Workbench Database Script
-- ============================================

-- Create Database
CREATE DATABASE IF NOT EXISTS NotesSystem;
USE NotesSystem;

-- ============================================
-- USERS TABLE (Customer Panel)
-- ============================================
CREATE TABLE IF NOT EXISTS Users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Phone VARCHAR(20) NULL,
    ProfileImageUrl VARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    INDEX idx_email (Email),
    INDEX idx_is_active (IsActive)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- NOTES TABLE
-- ============================================
CREATE TABLE IF NOT EXISTS Notes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(200) NOT NULL,
    Content TEXT NOT NULL,
    ImageUrl VARCHAR(500) NULL,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME NULL,
    IsImportant BOOLEAN DEFAULT FALSE,
    UserId INT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    INDEX idx_user_id (UserId),
    INDEX idx_created_at (CreatedAt)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ============================================
-- INSERT USERS QUERY
-- ============================================
INSERT INTO Users (FullName, Email, Password, Phone, ProfileImageUrl, CreatedAt, IsActive) VALUES
('John Smith', 'john@example.com', 'password123', '9876543210', 'https://randomuser.me/api/portraits/men/1.jpg', '2024-06-15 09:30:00', TRUE),
ON DUPLICATE KEY UPDATE FullName = FullName;

-- ============================================
-- INSERT NOTES QUERY
-- ============================================
INSERT INTO Notes (Title, Content, ImageUrl, UserId, IsImportant, CreatedAt) VALUES
('Quick Reminder', 'Call mom today.', NULL, 1, FALSE, '2024-06-20 10:00:00'),
('Grocery', 'Buy milk and bread.', NULL, 1, FALSE, '2024-06-25 09:15:00'),
('Todo', 'Finish report by 5pm.', NULL, 1, TRUE, '2024-07-01 08:30:00'),

INSERT INTO Notes (Title, Content, ImageUrl, UserId, IsImportant, CreatedAt) VALUES
('Weekly Goals', 'This week I need to complete the project proposal, attend two client meetings, and finalize the budget report. Also need to review team performance and schedule one-on-ones.', 'https://images.unsplash.com/photo-1484480974693-6ca0a78fb36b?w=400', 1, TRUE, '2024-07-05 09:00:00'),
('Recipe Notes', 'Grandmas pasta recipe: Boil pasta for 8 mins, saute garlic in olive oil, add tomatoes and basil. Simmer for 15 minutes. Season with salt and pepper. Top with parmesan cheese.', 'https://images.unsplash.com/photo-1551183053-bf91a1d81141?w=400', 1, FALSE, '2024-07-20 18:30:00'),
('Book Summary', 'Atomic Habits key points: 1% better every day compounds. Focus on systems not goals. Make good habits obvious and easy. Make bad habits invisible and hard. Identity shapes behavior.', 'https://images.unsplash.com/photo-1512820790803-83ca734da794?w=400', 1, TRUE, '2024-08-10 21:00:00'),

INSERT INTO Notes (Title, Content, ImageUrl, UserId, IsImportant, CreatedAt) VALUES
('Annual Goals 2024', 'This year I am committed to achieving several important milestones. First, I want to advance my career by completing professional certifications and taking on more leadership responsibilities at work. Second, I plan to improve my health by exercising at least four times per week, eating more vegetables, and getting seven to eight hours of sleep consistently. Third, I aim to save 25% of my income and invest wisely for the future. Fourth, I want to strengthen relationships by spending quality time with family and making new friends. Finally, I will dedicate time to personal growth through reading, learning new skills, and practicing mindfulness. I will review these goals quarterly and adjust as needed.', 'https://images.unsplash.com/photo-1484480974693-6ca0a78fb36b?w=400', 1, TRUE, '2024-06-18 08:00:00'),
('Project Specification', 'The new customer portal project requires careful planning and execution. The scope includes user authentication with multi-factor support, a responsive dashboard for account management, integration with existing CRM systems, real-time notifications, document upload and management, payment processing capabilities, and comprehensive reporting features. The tech stack will consist of React for the frontend, Node.js with Express for the backend, PostgreSQL for the database, and Redis for caching. We need to complete the MVP within twelve weeks, with phase two enhancements following. The team consists of three frontend developers, two backend developers, one DevOps engineer, and one QA specialist.', 'https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400', 1, TRUE, '2024-07-03 10:30:00'),
('Research Findings', 'After conducting extensive market research on consumer behavior trends, several key insights have emerged. First, mobile shopping continues to grow rapidly, with 73% of consumers now preferring to browse and purchase via smartphones. Second, sustainability and ethical sourcing are increasingly important factors in purchasing decisions especially among younger demographics. Third, personalization significantly impacts customer loyalty with personalized recommendations driving 35% higher conversion rates. Fourth, same-day delivery expectations are becoming the norm in urban areas. Fifth, social media influence on purchase decisions has doubled in the past two years. Based on these findings I recommend we prioritize mobile optimization and highlight our sustainability initiatives.', 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400', 1, TRUE, '2024-07-22 14:45:00'),
