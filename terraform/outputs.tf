output "resource_group_name" {
  description = "Name of the resource group"
  value       = azurerm_resource_group.main.name
}

output "acr_login_server" {
  description = "ACR login server URL"
  value       = azurerm_container_registry.main.login_server
}

output "acr_registry_username" {
  description = "Username for the container registry"
  value       = azurerm_container_registry.main.admin_username
  sensitive   = true
}

output "acr_registry_password" {
  description = "Password for the container registry"
  value       = azurerm_container_registry.main.admin_password
  sensitive   = true
}

output "acr_name" {
  description = "ACR name"
  value       = azurerm_container_registry.main.name
}

output "container_app_url" {
  description = "URL of the container app"
  value       = "https://${azurerm_container_app.api.latest_revision_fqdn}"
}

output "container_app_name" {
  description = "Name of the container app"
  value       = azurerm_container_app.api.name
}