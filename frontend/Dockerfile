FROM node:22-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm ci

COPY . .

ARG VITE_AUTH_API
ARG VITE_RESULTS_API
ARG VITE_ACH_API
ENV VITE_AUTH_API=$VITE_AUTH_API
ENV VITE_RESULTS_API=$VITE_RESULTS_API
ENV VITE_ACH_API=$VITE_ACH_API

RUN npm run build

FROM nginx:1.27-alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist ./

EXPOSE 80
