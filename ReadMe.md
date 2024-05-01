# Remote device Managment
This repository is showing a typical device managment from distance.
In this project we have many devices around the world that they are publishing the measurements to an MQTT hub, while in the control room we can read sensor values, add a new sensor, start and stop devices.
These imaginary devices are identified/distinguished by their Id. each sensor is producing Speed, Battery, Temperature and Humidity values. Once they receive start command they produce message as long as they have battery.

## How is works
In this project Microsoft ASP.NET Core is used as Backend and React/Vite is used as Frontend tool.


## What is ready

## Tasks
You can choose any of following tasks
* Backend
    * Start sensor command
    * Stop sensor command
* Frontend
    * Showing Sensor values
    * Adding a new sensor