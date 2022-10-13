if [ $(docker ps -a --format {{.Names}} | grep webapp-test) ]
then
    docker rm -f webapp-test
    docker rmi gitlab.umec.com.tw:8443/images/webapp-mealwebquery-test:latest 
fi

if [ $(docker ps -a --format {{.Names}} | grep nginx-test) ]
then
    docker rm -f nginx-test
    docker rmi gitlab.umec.com.tw:8443/images/webapp-mealwebquery-nginx-test:latest 
fi

if [ $(docker ps -a --format {{.Names}} | grep busybox-test) ]
then
    docker rm -f busybox-test
    docker rmi sequenceiq/busybox 
fi
