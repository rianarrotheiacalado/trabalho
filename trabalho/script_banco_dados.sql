CREATE TABLE "Salas" (
    "Id" SERIAL PRIMARY KEY,
    "Nome" VARCHAR(100) NOT NULL,
    "Andar" INT NOT NULL,
    "QuantidadeAssentos" INT NOT NULL
);

CREATE TABLE "Reservas" (
    "Id" SERIAL PRIMARY KEY,
    "Inicio" TIMESTAMP NOT NULL,
    "Fim" TIMESTAMP NOT NULL,
    "SalaId" INT NOT NULL,
    CONSTRAINT "FK_Reservas_Salas_SalaId" FOREIGN KEY ("SalaId") REFERENCES "Salas" ("Id") ON DELETE CASCADE
);