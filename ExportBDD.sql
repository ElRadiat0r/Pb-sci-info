CREATE DATABASE  IF NOT EXISTS `livinparis` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `livinparis`;
-- MySQL dump 10.13  Distrib 8.0.40, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: livinparis
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `avis`
--

DROP TABLE IF EXISTS `avis`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `avis` (
  `id_avis` int NOT NULL AUTO_INCREMENT,
  `id_commande` int NOT NULL,
  `id_client` int NOT NULL,
  `id_cuisinier` int NOT NULL,
  `etoile` int DEFAULT NULL,
  `commentaire` varchar(2000) DEFAULT NULL,
  `date_avis` datetime DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_avis`),
  KEY `id_commande` (`id_commande`),
  KEY `id_client` (`id_client`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `avis_ibfk_1` FOREIGN KEY (`id_commande`) REFERENCES `commande` (`id_commande`) ON DELETE CASCADE,
  CONSTRAINT `avis_ibfk_2` FOREIGN KEY (`id_client`) REFERENCES `utilisateur` (`id_utilisateur`),
  CONSTRAINT `avis_ibfk_3` FOREIGN KEY (`id_cuisinier`) REFERENCES `utilisateur` (`id_utilisateur`),
  CONSTRAINT `avis_chk_1` CHECK ((`etoile` between 1 and 5))
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `avis`
--

LOCK TABLES `avis` WRITE;
/*!40000 ALTER TABLE `avis` DISABLE KEYS */;
INSERT INTO `avis` VALUES (1,1,1,3,5,'Un r√©gal !','2025-04-09 17:52:18'),(2,1,2,4,4,'D√©licieux.','2025-04-09 17:52:18'),(3,2,5,6,3,'Mon plat √©tait un peu froid.','2025-04-09 17:52:18');
/*!40000 ALTER TABLE `avis` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `commande`
--

DROP TABLE IF EXISTS `commande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `commande` (
  `id_commande` int NOT NULL AUTO_INCREMENT,
  `id_client` int NOT NULL,
  `date_commande` datetime NOT NULL,
  `montant_total` decimal(10,2) NOT NULL,
  PRIMARY KEY (`id_commande`),
  KEY `id_client` (`id_client`),
  CONSTRAINT `commande_ibfk_1` FOREIGN KEY (`id_client`) REFERENCES `utilisateur` (`id_utilisateur`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES (1,1,'2025-04-01 12:00:00',46.50),(2,2,'2025-04-02 14:00:00',77.50),(3,3,'2025-04-10 12:17:50',68.00),(4,5,'2025-04-04 18:00:00',97.50),(5,7,'2025-04-05 20:00:00',32.00),(6,8,'2025-04-06 11:30:00',48.50),(7,9,'2025-04-07 14:30:00',73.00),(8,10,'2025-04-08 16:00:00',95.50),(9,11,'2025-04-09 10:45:00',45.50),(10,3,'2025-04-10 13:15:00',58.00),(11,12,'2025-04-10 12:02:28',17.00);
/*!40000 ALTER TABLE `commande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `ingredient`
--

DROP TABLE IF EXISTS `ingredient`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `ingredient` (
  `id_ingredient` int NOT NULL AUTO_INCREMENT,
  `nom` varchar(100) NOT NULL,
  PRIMARY KEY (`id_ingredient`),
  UNIQUE KEY `nom` (`nom`)
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (6,'Champignons'),(8,'Chocolat'),(7,'Cr√®me fra√Æche'),(3,'Fromage'),(11,'Lait'),(10,'Lardons'),(12,'Oeufs'),(9,'P√¢tes'),(2,'P√¢tes √† lasagnes'),(14,'Pommes'),(4,'Poulet'),(5,'Riz'),(13,'Sucre'),(1,'Tomates');
/*!40000 ALTER TABLE `ingredient` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lignecommande`
--

DROP TABLE IF EXISTS `lignecommande`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lignecommande` (
  `id_ligne` int NOT NULL AUTO_INCREMENT,
  `id_commande` int NOT NULL,
  `id_plat` int NOT NULL,
  `quantite` int NOT NULL,
  PRIMARY KEY (`id_ligne`),
  KEY `id_commande` (`id_commande`),
  KEY `id_plat` (`id_plat`),
  CONSTRAINT `lignecommande_ibfk_1` FOREIGN KEY (`id_commande`) REFERENCES `commande` (`id_commande`) ON DELETE CASCADE,
  CONSTRAINT `lignecommande_ibfk_2` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`)
) ENGINE=InnoDB AUTO_INCREMENT=28 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lignecommande`
--

LOCK TABLES `lignecommande` WRITE;
/*!40000 ALTER TABLE `lignecommande` DISABLE KEYS */;
INSERT INTO `lignecommande` VALUES (1,1,1,2),(2,1,3,1),(3,1,5,1),(4,2,2,3),(5,2,6,2),(6,2,4,3),(9,4,1,3),(10,4,2,2),(11,4,7,4),(12,5,4,2),(13,5,6,2),(14,6,5,3),(15,6,7,2),(16,7,2,4),(17,7,5,2),(18,8,1,4),(19,8,6,3),(20,8,7,2),(21,9,4,3),(22,9,2,2),(23,10,3,3),(24,10,1,2),(25,11,7,2),(26,3,5,4),(27,3,4,4);
/*!40000 ALTER TABLE `lignecommande` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `plat`
--

DROP TABLE IF EXISTS `plat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `plat` (
  `id_plat` int NOT NULL AUTO_INCREMENT,
  `id_cuisinier` int NOT NULL,
  `nom_plat` varchar(100) NOT NULL,
  `date_fabrication` date NOT NULL,
  `prix_plat` decimal(8,2) NOT NULL,
  `nationalite` varchar(50) NOT NULL,
  `regime_alimentaire` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_plat`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `plat_ibfk_1` FOREIGN KEY (`id_cuisinier`) REFERENCES `utilisateur` (`id_utilisateur`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
INSERT INTO `plat` VALUES (1,4,'Lasagnes √† la bolognaise','2025-04-01',12.50,'Italienne','Omnivore'),(2,3,'Poulet √† la cr√®me et champignons','2025-04-02',13.00,'Fran√ßaise','Omnivore'),(3,3,'Riz au poulet fa√ßon tha√Ø','2025-04-01',11.00,'Tha√Ølandaise','Omnivore'),(4,4,'G√¢teau au chocolat','2025-04-01',6.50,'Fran√ßaise','V√©g√©tarien'),(5,4,'P√¢tes carbonara','2025-04-01',10.50,'Italienne','Omnivore'),(6,6,'Omelette aux lardons','2025-04-01',9.50,'Fran√ßaise','Omnivore'),(7,8,'Tartes aux pommes','2025-04-01',8.50,'Fran√ßaise','V√©g√©tarien');
/*!40000 ALTER TABLE `plat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `platingredients`
--

DROP TABLE IF EXISTS `platingredients`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `platingredients` (
  `id_plat` int NOT NULL,
  `id_ingredient` int NOT NULL,
  PRIMARY KEY (`id_plat`,`id_ingredient`),
  KEY `id_ingredient` (`id_ingredient`),
  CONSTRAINT `platingredients_ibfk_1` FOREIGN KEY (`id_plat`) REFERENCES `plat` (`id_plat`) ON DELETE CASCADE,
  CONSTRAINT `platingredients_ibfk_2` FOREIGN KEY (`id_ingredient`) REFERENCES `ingredient` (`id_ingredient`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `platingredients`
--

LOCK TABLES `platingredients` WRITE;
/*!40000 ALTER TABLE `platingredients` DISABLE KEYS */;
INSERT INTO `platingredients` VALUES (1,1),(1,3),(5,3),(1,4),(5,4),(1,5),(2,5),(3,5),(5,5),(2,7),(2,8),(4,8),(3,9),(5,10),(6,10),(4,11),(6,11),(7,11),(4,12),(6,12),(4,13),(7,13),(7,14);
/*!40000 ALTER TABLE `platingredients` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `utilisateur`
--

DROP TABLE IF EXISTS `utilisateur`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `utilisateur` (
  `id_utilisateur` int NOT NULL AUTO_INCREMENT,
  `prenom` varchar(50) NOT NULL,
  `nom` varchar(50) NOT NULL,
  `coordonnees_geographiques` point NOT NULL,
  `email` varchar(100) NOT NULL,
  `mdp` varchar(255) NOT NULL,
  `est_client` tinyint(1) DEFAULT '0',
  `est_cuisinier` tinyint(1) DEFAULT '0',
  `type_client` enum('Particulier','Entreprise') DEFAULT NULL,
  PRIMARY KEY (`id_utilisateur`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=13 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
INSERT INTO `utilisateur` VALUES (1,'Alice','Durand',_binary '\0\0\0\0\0\0\0®5\Õ;N\—@v\‡ú•mH@','alice.durand@example.com','mdp123',1,0,'Particulier'),(2,'Bob','Martin',_binary '\0\0\0\0\0\0\0£í:M@fffffnH@','bob.martin@example.com','mdp456',1,0,'Entreprise'),(3,'Clara','Lemoine',_binary '\0\0\0\0\0\0\0\ZQ\⁄|\·@wæü\Z/mH@','clara.lemoine@example.com','mdp789',1,1,'Particulier'),(4,'David','Bernard',_binary '\0\0\0\0\0\0\0\\è\¬ı(\\@Vü´≠\ÿoH@','david.bernard@example.com','pass001',0,1,NULL),(5,'Emma','Morel',_binary '\0\0\0\0\0\0\0Ö\ÎQ∏@yX®5\ÕkH@','emma.morel@example.com','pass002',1,0,'Entreprise'),(6,'Fabien','Roux',_binary '\0\0\0\0\0\0\0§p=\n◊£@è\¬ı(\\oH@','fabien.roux@example.com','pass003',0,1,NULL),(7,'Claire','Dupont',_binary '\0\0\0\0\0\0\0\Ì\ræ0ô™@ÆG\·znH@','claire.dupont@example.com','mdp123',1,0,'Particulier'),(8,'Julien','Martin',_binary '\0\0\0\0\0\0\0∏Ö\ÎQ∏@è\¬ı(\\oH@','julien.martin@example.com','mdp456',1,1,'Entreprise'),(9,'Sophie','Bernard',_binary '\0\0\0\0\0\0\0ˆ(\\è\¬ı@\Õ\Ã\Ã\Ã\ÃlH@','sophie.bernard@example.com','mdp789',1,0,'Particulier'),(10,'Lucas','Moreau',_binary '\0\0\0\0\0\0\0{ÆG\·z@Ö\ÎQ∏nH@','lucas.moreau@example.com','mdp321',1,1,'Entreprise'),(11,'Emma','Leroy',_binary '\0\0\0\0\0\0\0◊£p=\n\◊@\ÏQ∏ÖkH@','emma.leroy@example.com','mdp654',1,0,'Particulier'),(12,'Alexandre','DUFORET',_binary '\0\0\0\0\0\0\0£í:M@fffffnH@','alexandre.duforet@edu.devinci.fr','vroum',1,1,'Particulier');
/*!40000 ALTER TABLE `utilisateur` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-04-10 12:40:52
