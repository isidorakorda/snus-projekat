@echo off

echo Starting minikube
minikube start --driver=docker
minikube addons enable ingress
 
echo Pointing docker to minikube
FOR /f "tokens=*" %%i IN ('minikube docker-env') DO %%i

echo Building images, this will take some time
cd..

docker build -t api-gateway:latest -f ApiGateway/Dockerfile .

docker build -t consensus-service:latest -f ConsensusService/Dockerfile .

docker build -t ingestion-service:latest -f Server/Dockerfile .

docker build -t sensor-registry:latest -f SensorRegistry/Dockerfile .

docker build -t sensor-simulation:latest -f SensorSimulator/Dockerfile .

docker build -t notification-service:latest -f NotificationService/Dockerfile .

echo Applying Services to k8s
cd configuration
cd k8s

kubectl apply -f ./namespaces -f ./ingress -f ./db -f ./api-gateway -f ./consensus-service -f ./ingestion-service -f ./registry-rbac -f ./sensor-registry  -f ./sensor-simulation -f ./notification-service 

echo Logs
kubectl get pods --all-namespaces


echo Pointing docker to normal
FOR /f "tokens=*" %%i IN ('minikube docker-env -u') DO %%i

minikube service ingress-nginx-controller -n ingress-nginx

echo Press anything to exit
pause >nul
