minio-start:
	docker run -d \
		--restart unless-stopped \
		--name minio \
		-p 9000:9000 \
		minio/minio server /data

minio-get-credentials:
	@echo "Minio credentials:"
	@docker exec -it minio cat /data/.minio.sys/config/config.json | grep accessKey | sed -e "s/\t//g"
	@docker exec -it minio cat /data/.minio.sys/config/config.json | grep secretKey | sed -e "s/\t//g"

project-build:
	dotnet run

project-publish:
	dotnet publish -c release --self-contained --runtime linux-x64

project-clean:
	rm -rf bin/
	rm -rf obj/
