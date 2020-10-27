set KAFKA_OPTS=-Djava.security.auth.login.config=N:\Workspace\Capstone\FQSC.Admin\FQCS.Admin\FQCS.Admin.ConsoleClient\kafka_server_jaas.conf
N:
cd N:\ITs\Kafka\kafka_2.12-2.4.1\bin\windows
kafka-server-start.bat ../../config/server.properties
