# storage

## Intro

storage service for Cloud Mall

## Docker

``` bash
# docker rmi cloudmall/storage:latest # remove before docker image if exists

docker run -p 5050:80 -v /etc/configs/storage.appsettings.json:/app/appsettings/Production.json cloudmall/storage:latest
```
