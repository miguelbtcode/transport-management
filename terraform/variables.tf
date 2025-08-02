variable "environment" {
  description = "Environment name"
  type        = string
}

variable "location" {
  description = "Azure region"
  type        = string
  default     = "East US"
}

variable "project_name" {
  description = "Project name"
  type        = string
  default     = "sigvehicular"
}

variable "container_image" {
  description = "Container image URL"
  type        = string
}

variable "database_connection_string" {
  description = "Database connection string"
  type        = string
  sensitive   = true
}

variable "jwt_key" {
  description = "JWT secret key"
  type        = string
  sensitive   = true
}

variable "jwt_issuer" {
  description = "JWT issuer"
  type        = string
  default     = "SigVehicular.Api"
}

variable "jwt_audience" {
  description = "JWT audience"
  type        = string
  default     = "SigVehicular.Client"
}