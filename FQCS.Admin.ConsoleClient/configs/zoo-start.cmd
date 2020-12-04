set KAFKA_OPTS=-Djava.security.auth.login.config=N:\Workspace\Capstone\FQSC.Admin\FQCS.Admin\FQCS.Admin.ConsoleClient\configs\zookeeper_jaas.conf
N:
cd N:\Servers\Kafka-Capstone\source\bin\windows
zookeeper-server-start.bat ../../config/zookeeper.properties
