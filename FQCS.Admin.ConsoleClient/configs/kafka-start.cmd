set KAFKA_OPTS=-Djava.security.auth.login.config=N:\Workspace\Capstone\FQSC.Admin\FQCS.Admin\FQCS.Admin.ConsoleClient\configs\kafka_server_jaas.conf
N:
cd N:\Servers\Kafka-Capstone\bin\windows
kafka-server-start.bat ../../config/server.properties
