FROM rabbitmq:4-management

# Expose the default RabbitMQ ports
# 5672 for AMQP
# 15672 for management UI
EXPOSE 5672 15672

# Optional: Set default environment variables
ENV RABBITMQ_DEFAULT_USER=admin
ENV RABBITMQ_DEFAULT_PASS=admin

# Optional: Enable management plugin (already enabled in management image)
# RUN rabbitmq-plugins enable rabbitmq_management