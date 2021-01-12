FROM postgres
EXPOSE 5432
ENV POSTGRES_USER docker
ENV POSTGRES_PASSWORD docker
ENV POSTGRES_DB stockTickers
COPY pgTableInit.sql /docker-entrypoint-initdb.d/