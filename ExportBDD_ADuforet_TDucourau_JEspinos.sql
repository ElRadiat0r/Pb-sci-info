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
INSERT INTO `avis` VALUES (1,1,1,2,5,'Excellentes lasagnes, un régal !','2025-03-26 16:58:44'),(2,1,1,3,4,'Tiramisu très bon, un peu sucré.','2025-03-26 16:58:44'),(3,2,4,2,3,'Poulet basquaise correct, manque un peu de sauce.','2025-03-26 16:58:44');
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
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `commande`
--

LOCK TABLES `commande` WRITE;
/*!40000 ALTER TABLE `commande` DISABLE KEYS */;
INSERT INTO `commande` VALUES (1,1,'2025-03-20 12:30:00',35.00),(2,2,'2025-03-21 18:45:00',42.50),(3,3,'2025-03-22 20:00:00',28.90),(4,4,'2025-03-23 13:15:00',15.75),(5,5,'2025-03-24 19:30:00',50.00),(7,7,'2025-03-26 14:45:00',18.90),(8,8,'2025-03-27 17:10:00',37.20),(9,9,'2025-03-28 19:50:00',60.00),(10,10,'2025-03-29 11:25:00',12.50),(11,11,'2025-03-30 12:30:00',45.00),(12,12,'2025-03-31 18:15:00',39.90),(13,13,'2025-04-01 20:45:00',32.10),(14,14,'2025-04-02 13:50:00',55.00),(15,15,'2025-04-03 19:05:00',48.75),(16,1,'2025-03-30 19:00:00',1080.00),(17,8,'2025-04-02 22:41:48',45.00),(18,2,'2025-04-02 22:45:00',8.00),(19,4,'2025-04-03 11:28:17',30.00),(20,3,'2025-04-03 20:39:11',69.00);
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
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `ingredient`
--

LOCK TABLES `ingredient` WRITE;
/*!40000 ALTER TABLE `ingredient` DISABLE KEYS */;
INSERT INTO `ingredient` VALUES (6,'Champignons'),(8,'Chocolat'),(7,'Crème fraîche'),(3,'Fromage'),(9,'Oeufs'),(2,'Pâtes à lasagnes'),(4,'Poulet'),(5,'Riz'),(10,'Sucre'),(1,'Tomates');
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
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lignecommande`
--

LOCK TABLES `lignecommande` WRITE;
/*!40000 ALTER TABLE `lignecommande` DISABLE KEYS */;
INSERT INTO `lignecommande` VALUES (1,1,1,2),(2,1,3,1),(3,2,2,3),(4,2,4,1),(5,3,1,1),(6,3,2,2),(7,4,3,1),(8,5,4,1),(10,7,2,3),(11,8,3,2),(12,9,4,1),(13,10,1,3),(14,11,2,1),(15,12,3,2),(16,13,4,1),(17,14,1,2),(18,15,2,1),(20,17,2,3),(21,18,3,1),(23,19,2,2),(24,16,9,108),(25,20,1,3),(26,20,4,3);
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
  `nb_personnes` int NOT NULL,
  `date_fabrication` date NOT NULL,
  `prix_par_personne` decimal(8,2) NOT NULL,
  `nationalite` varchar(50) NOT NULL,
  `regime_alimentaire` varchar(50) DEFAULT NULL,
  PRIMARY KEY (`id_plat`),
  KEY `id_cuisinier` (`id_cuisinier`),
  CONSTRAINT `plat_ibfk_1` FOREIGN KEY (`id_cuisinier`) REFERENCES `utilisateur` (`id_utilisateur`)
) ENGINE=InnoDB AUTO_INCREMENT=11 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `plat`
--

LOCK TABLES `plat` WRITE;
/*!40000 ALTER TABLE `plat` DISABLE KEYS */;
INSERT INTO `plat` VALUES (1,2,'Lasagnes végétariennes',4,'2025-03-26',12.50,'Italienne','Végétarien'),(2,2,'Poulet basquaise',2,'2025-03-26',15.00,'Française',NULL),(3,3,'Tiramisu maison',3,'2025-03-26',8.00,'Italienne',NULL),(4,3,'Riz cantonais',4,'2025-03-26',10.50,'Chinoise',NULL),(9,2,'Quiche lorraine',4,'2025-03-29',10.00,'Française',NULL),(10,4,'Tarte aux poireaux',6,'2025-03-31',11.20,'Française','Végétarien');
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
INSERT INTO `platingredients` VALUES (1,1),(2,1),(1,2),(1,3),(2,4),(4,5),(2,6),(3,8),(3,9),(3,10);
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
  `adresse` varchar(255) NOT NULL,
  `code_postal` varchar(10) NOT NULL,
  `email` varchar(100) NOT NULL,
  `mdp` varchar(255) NOT NULL,
  `est_client` tinyint(1) DEFAULT '0',
  `est_cuisinier` tinyint(1) DEFAULT '0',
  `type_client` enum('Particulier','Entreprise') DEFAULT NULL,
  PRIMARY KEY (`id_utilisateur`),
  UNIQUE KEY `email` (`email`)
) ENGINE=InnoDB AUTO_INCREMENT=18 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `utilisateur`
--

LOCK TABLES `utilisateur` WRITE;
/*!40000 ALTER TABLE `utilisateur` DISABLE KEYS */;
INSERT INTO `utilisateur` VALUES (1,'Alexandre','Duforet','1 Place Vendôme','75001','alexandre.duforet@edu.devinci.fr','aberrant',1,0,'Particulier'),(2,'Jean','Sion','21 Avenue du Go Fast','75007','jean.sion@edu.devinci.fr','les',1,1,'Particulier'),(3,'Steve','Jobs','One Apple Park Way Cupertino','75000','steve.jobs@apple.com','onemorething',1,1,'Particulier'),(4,'Alice','Martin','24 Boulevard Haussmann','75008','alice.martin@email.com','mdp789',1,1,'Particulier'),(5,'Paul','Durand','7 Rue Lafayette','75010','paul.durand@email.com','mdp321',1,0,'Entreprise'),(7,'Camille','Dubois','3 Rue Mouffetard','75005','camille.dubois@email.com','mdp987',1,0,'Entreprise'),(8,'Sophie','Lemoine','9 Avenue Foch','75016','sophie.lemoine@email.com','mdp741',1,0,'Particulier'),(9,'Antoine','Morel','21 Rue de Rivoli','75004','antoine.morel@email.com','mdp852',1,0,'Entreprise'),(10,'Manon','Petit','14 Quai de la Loire','75019','manon.petit@email.com','mdp963',1,0,'Particulier'),(11,'Hugo','Robert','8 Rue du Commerce','75015','hugo.robert@email.com','mdp159',1,0,'Entreprise'),(12,'Emma','Richard','11 Rue Saint-Honoré','75001','emma.richard@email.com','mdp753',1,0,'Particulier'),(13,'Nathan','Garcia','25 Rue de Belleville','75020','nathan.garcia@email.com','mdp951',1,0,'Entreprise'),(14,'Clara','Martinez','30 Boulevard Voltaire','75011','clara.martinez@email.com','mdp357',1,0,'Particulier'),(15,'Maxime','Fernandez','6 Avenue Montaigne','75008','maxime.fernandez@email.com','mdp258',1,0,'Entreprise'),(16,'Julie','Lefebvre','15 Rue du Faubourg Saint-Antoine','75012','julie.lefebvre@email.com','mdp753',1,0,'Particulier'),(17,'Matéo','Dubreuil','13 Rue Marceau','75015','mateo.dubreuil@edu.devinci.fr','route',1,1,'Particulier');
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

-- Dump completed on 2025-04-03 20:46:14
