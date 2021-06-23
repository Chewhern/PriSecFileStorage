-- phpMyAdmin SQL Dump
-- version 4.9.5deb2
-- https://www.phpmyadmin.net/
--
-- Host: localhost:3306
-- Generation Time: Jun 23, 2021 at 07:54 AM
-- Server version: 8.0.25-0ubuntu0.20.04.1
-- PHP Version: 7.4.3

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET AUTOCOMMIT = 0;
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `PriSecFileStorage`
--

-- --------------------------------------------------------

--
-- Table structure for table `Purchase_Records`
--

CREATE TABLE `Purchase_Records` (
  `ID` varchar(500) NOT NULL,
  `Expiration_Date` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `Random_Challenge`
--

CREATE TABLE `Random_Challenge` (
  `User_ID` varchar(500) NOT NULL,
  `Challenge` text NOT NULL,
  `Authentication_Type` text NOT NULL,
  `Valid_Duration` datetime DEFAULT NULL,
  `ID` varchar(500) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- --------------------------------------------------------

--
-- Table structure for table `User`
--

CREATE TABLE `User` (
  `User_ID` varchar(500) NOT NULL,
  `Login_Signed_PK` text NOT NULL,
  `Login_PK` text NOT NULL,
  `Ciphered_Recovery_Data` text NOT NULL,
  `Lock_Status` varchar(500) DEFAULT NULL,
  `Lock_Till` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Indexes for dumped tables
--

--
-- Indexes for table `Purchase_Records`
--
ALTER TABLE `Purchase_Records`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `Random_Challenge`
--
ALTER TABLE `Random_Challenge`
  ADD PRIMARY KEY (`ID`);

--
-- Indexes for table `User`
--
ALTER TABLE `User`
  ADD PRIMARY KEY (`User_ID`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
