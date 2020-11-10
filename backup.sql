-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: publishwritermnr
-- ------------------------------------------------------
-- Server version	8.0.19

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `account`
--

DROP TABLE IF EXISTS `account`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `account` (
  `id` char(12) NOT NULL,
  `pw` char(12) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `account`
--

LOCK TABLES `account` WRITE;
/*!40000 ALTER TABLE `account` DISABLE KEYS */;
INSERT INTO `account` VALUES ('0','0'),('1','1'),('123','123'),('2','2'),('3','3'),('4','4'),('aaa','333'),('asa','222');
/*!40000 ALTER TABLE `account` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `book`
--

DROP TABLE IF EXISTS `book`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `book` (
  `BH` char(12) NOT NULL,
  `name` char(10) NOT NULL,
  `time` char(12) DEFAULT NULL,
  `price` float DEFAULT NULL,
  `id` varchar(45) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `book`
--

LOCK TABLES `book` WRITE;
/*!40000 ALTER TABLE `book` DISABLE KEYS */;
INSERT INTO `book` VALUES ('1002','时间简史','2020.2.12',45.9,'aaa',1),('1005','FantasyLi','2020.9.13',0,'aaa',3),('100','233','发行时间',0,'123',9);
/*!40000 ALTER TABLE `book` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `department`
--

DROP TABLE IF EXISTS `department`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `department` (
  `BMH` char(12) NOT NULL,
  `name` char(10) NOT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `department`
--

LOCK TABLES `department` WRITE;
/*!40000 ALTER TABLE `department` DISABLE KEYS */;
INSERT INTO `department` VALUES ('001','科学部','aaa',1),('002','人文部','aaa',2),('003','历史部','aaa',9);
/*!40000 ALTER TABLE `department` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doweiwen`
--

DROP TABLE IF EXISTS `doweiwen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doweiwen` (
  `name` char(12) NOT NULL,
  `SBH` char(12) NOT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doweiwen`
--

LOCK TABLES `doweiwen` WRITE;
/*!40000 ALTER TABLE `doweiwen` DISABLE KEYS */;
INSERT INTO `doweiwen` VALUES ('0022','时间','aaa',1),('名称','时间','aaa',2);
/*!40000 ALTER TABLE `doweiwen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `doyantao`
--

DROP TABLE IF EXISTS `doyantao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `doyantao` (
  `name` char(12) NOT NULL,
  `SBH` char(12) NOT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `doyantao`
--

LOCK TABLES `doyantao` WRITE;
/*!40000 ALTER TABLE `doyantao` DISABLE KEYS */;
INSERT INTO `doyantao` VALUES ('000','怎么写书','aaa',1);
/*!40000 ALTER TABLE `doyantao` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `weiwen`
--

DROP TABLE IF EXISTS `weiwen`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `weiwen` (
  `name` char(10) NOT NULL,
  `time` char(12) DEFAULT NULL,
  `address` char(10) DEFAULT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `weiwen`
--

LOCK TABLES `weiwen` WRITE;
/*!40000 ALTER TABLE `weiwen` DISABLE KEYS */;
INSERT INTO `weiwen` VALUES ('003','时间','地点','aaa',1),('第一次慰问','2020.9.30','Room1','aaa',3),('第二次慰问','2020.10.24','2655','aaa',10);
/*!40000 ALTER TABLE `weiwen` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `writebook`
--

DROP TABLE IF EXISTS `writebook`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `writebook` (
  `BBH` char(12) NOT NULL,
  `SBH` char(12) NOT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `writebook`
--

LOCK TABLES `writebook` WRITE;
/*!40000 ALTER TABLE `writebook` DISABLE KEYS */;
INSERT INTO `writebook` VALUES ('1002','006','aaa',1),('100','012','123',2);
/*!40000 ALTER TABLE `writebook` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `writer`
--

DROP TABLE IF EXISTS `writer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `writer` (
  `BH` char(12) NOT NULL,
  `name` char(12) DEFAULT NULL,
  `sex` char(2) DEFAULT NULL,
  `birth` char(18) DEFAULT NULL,
  `BMH` char(12) DEFAULT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=46 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `writer`
--

LOCK TABLES `writer` WRITE;
/*!40000 ALTER TABLE `writer` DISABLE KEYS */;
INSERT INTO `writer` VALUES ('001','牛三','男','2020.12.12','202','aaa',1),('003','李萧','女','2000.2.2','101','aaa',3),('005','黄月','女','1992.4.5','301','aaa',4),('006','阿奇','女','1999.8.8','201','aaa',6),('007','小七','男','1998.2.18','202','aaa',8),('012','233','性别','生日','部门号','123',45);
/*!40000 ALTER TABLE `writer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `yantao`
--

DROP TABLE IF EXISTS `yantao`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `yantao` (
  `name` char(10) NOT NULL,
  `Content` char(200) DEFAULT NULL,
  `time` char(12) DEFAULT NULL,
  `address` char(40) DEFAULT NULL,
  `id` char(12) DEFAULT NULL,
  `PK` int NOT NULL AUTO_INCREMENT,
  PRIMARY KEY (`PK`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=gb2312;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `yantao`
--

LOCK TABLES `yantao` WRITE;
/*!40000 ALTER TABLE `yantao` DISABLE KEYS */;
INSERT INTO `yantao` VALUES ('001','内容','时间','地点','aaa',1),('不知名研讨大会','一些神秘的不知道如何解释的怪异的想不透的傻B问题讨论','2020.9.23','ROOM101','aaa',2);
/*!40000 ALTER TABLE `yantao` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2020-11-10 12:15:16
