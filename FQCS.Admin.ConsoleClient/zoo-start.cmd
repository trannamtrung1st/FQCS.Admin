set KAFKA_OPTS=-Djava.security.auth.login.config=N:\Workspace\Capstone\FQSC.Admin\FQCS.Admin\FQCS.Admin.ConsoleClient\zookeeper_jaas.conf
N:
cd N:\Servers\Kafka-Capstone\bin\windows
zookeeper-server-start.bat ../../config/zookeeper.properties
