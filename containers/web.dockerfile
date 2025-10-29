FROM node:24

WORKDIR /app

RUN npm install -g pm2 serve

COPY app/ether-gizmos.shipyard.web/browser/ ./browser
RUN cp ./browser/assets/config.json ./browser/assets/config.base.json

COPY files/apply-config-env.js /opt/apply-config-env.js
COPY files/docker-entrypoint.web.sh /docker-entrypoint.sh
RUN sed -i 's/\r$//' /docker-entrypoint.sh /opt/apply-config-env.js \
  && chmod +x /docker-entrypoint.sh

EXPOSE 8080

ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["pm2", "serve", "/app/browser", "--no-daemon", "--spa"]
