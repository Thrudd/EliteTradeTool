# Thrudds Elite Trading Tool

This is a reupload of the source files for the elitetradingtool.co.uk site that ran from the Elite Dangerous Alpha in 2014 until October in 2017. I was unable to devote the time to refactor the code and revitalise the site in 2016/2017 so mothballed the project. Therfore the site is obsolite and trade in Elite is better served by newer tools that came along a few years after launch. I offer this code on the off chance that anyone finds it useful or wants to tinker with it.

The Microsoft SQL database is too large to upload to GitHub so I've made a zip of it available via Google Drive.
https://drive.google.com/file/d/1ITWEiPyDvB-k2GP-j-74Rug9M0A_d7d8/view?usp=sharing

## Project Structure
The system consited of three projects. The main website in EliteTrading, the EDDNService project for monitoring the EDDN network and updating the database and a common project of EliteTrading.Entities defining the database entities that both used.

## Technologies Used
The site is written in C# .net 4.5.1, jQuery 2.1.4 and Microsoft SQL Server 2008
