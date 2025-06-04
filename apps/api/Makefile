init:
	docker compose up -d --build

up:
	docker compose up -d

down:
	docker compose down

migrate:
	docker compose exec api dotnet ef database update

migrate-add:
	docker compose exec api dotnet ef migrations add $(name)

migrate-remove:
	docker compose exec api dotnet ef migrations remove

migrate-list:
	docker compose exec api dotnet ef migrations list