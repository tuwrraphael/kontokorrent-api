docker container stop kontokorrent-api
docker container rm kontokorrent-api
docker build --progress=plain --file Dockerfile -t kontokorrent-api .\
&& docker run \
        --user=$(id -u):$(id -g) \
        -v /mnt/App_Data:/App/wwwroot/App_Data \
        -v /mnt/appsettings.json:/App/appsettings.json \
        -p 8080:8080 \
        --name kontokorrent-api \
        -e ASPNETCORE_URLS=http://+:8080 \
        -d --restart always kontokorrent-api