# Overview

The case study demonstrate the use of [Open Weather Maps](https://openweathermap.org/api) through the api gateway provided my Kong Api manager.

The Open Weather Maps api have been registered with kong and an Api is created that calls the Kong admin api. Internally in Kong the OpenWeatherMaps api is called.

# Scope of Projects

In the Github repo the follows projects are present:

## WeatherApi
 An api that exposes an endpoint to get weather information by entering the latitude and longitude. This internally calls the kongapi route for OpenWeatherMaps api.

## Adapter.KongGateway
 This project contains all the information related to kong and it's admin api's to set up various services, routes, customers, plugins.
 
 This is done in order to achieve a sense of platform agnostic behaviour towards the choice of API Manager

 ## ApiProvisioner.CLI
 This projects sets up the environment inside kong, the purpose is to set up all the external apis from the provided configuration. 
 OpenWeatherMaps is one api but the setup has been made to accomodate any number of external apis.

 # Setup

 1. Assuming Docker is running, "curl -Ls https://get.konghq.com/quickstart | bash" command starts the kong server and supported database.

 2. Running ApiProvisioner.CLI project would setup the OpenWeatherMaps as a service in Kong.

 3. http://localhost:8002 can be accessed and will show API services that are registered with the local setup of Kong.

 4. Running weather Api is possible at this point and Will open are Swagger interface.

 # Tests

 An Automated test collection from "postman Weather_Gateway.postman_collection.json" can be open using Postman and all the tests are runable under the "Tests" section.

 Running the collection all together will run all the tests associated.

 # ApiKeys

 Api for the OpenWeatherMaps is not provided here. It is shared with the email informing about the case study. 
 The Api key needs to be put in WeatherApi/properties/launchSettings.json

