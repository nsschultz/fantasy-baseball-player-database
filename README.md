## Player Database Service
* This will become the source of truth for player data.
* Currently it is just a backup of the data.
* Part of becoming the source of truth will include exposing more endpoints that the CSV file does not.

---
### Endpoints:
* `GET api/player` - Gets all of the players from the database.
* `POST api/player` - Upserts the database with the collection of players provided.

---
### Healthcheck:
* The service will fail a healthcheck if database cannot be accessed. 
