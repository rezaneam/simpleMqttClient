# Remote device Managment
This repository is showing a typical device managment from distance.
In this project we have many devices around the world that they are publishing the measurements to a central communication hub (here we used MQTT technology), while in the control room we can read sensor values, add a new sensor, start and stop devices.
These imaginary devices are identified/distinguished by their Id. each sensor is producing Speed, Battery, Temperature and Humidity values. Once they receive start command they produce message as long as they have battery.

## How is works
In this project Microsoft ASP.NET Core is used as Backend and React/Vite is used as Frontend tool.


## What is ready
In this project most of fundation is done meaning that, the device manager is already connecting the the central hub and listens to the messages and can push messages.
In addition there is two REST full endpoints to read device values as well as adding new device.
On Frontend side not very much is done except establishing to the SignalR hub and listening to the messages but logging them in the console.

## Tasks
You can choose any of following tasks
* Backend (ASP.NET C#)
    * Start sensor command
    * Stop sensor command
* Frontend (React)
    * Showing Sensor values
    * Adding a new sensor