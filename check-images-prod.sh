if [ $(docker ps -a --format {{.Names}} | grep webapp-prod) ]
then
    docker rm -f webapp-prod
    docker rmi gitlab.umec.com.tw:8443/images/webapp-mealwebquery-prod:latest
fi

if [ $(docker ps -a --format {{.Names}} | grep nginx-prod) ]
then
    docker rm -f nginx-prod
    docker rmi gitlab.umec.com.tw:8443/images/webapp-mealwebquery-nginx-prod:latest 
fi

if [ $(docker ps -a --format {{.Names}} | grep busybox-prod) ]
then
    docker rm -f busybox-prod
    docker rmi sequenceiq/busybox:latest
fi
