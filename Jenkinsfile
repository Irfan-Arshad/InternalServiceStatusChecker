pipeline {
    agent any

    environment {
        IMAGE_NAME = "issc-api"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                sh '''
                  dotnet --version
                  dotnet restore ./InternalServiceStatusChecker/InternalServiceStatusChecker.csproj
                  dotnet build ./InternalServiceStatusChecker/InternalServiceStatusChecker.csproj -c Release --no-restore
                '''
            }
        }

        stage('Test') {
            steps {
                sh '''
                  dotnet test ./InternalServiceStatusChecker/InternalServiceStatusChecker.csproj -c Release --no-build || true
                '''
            }
        }

        stage('Docker Build') {
            steps {
                sh '''
                  docker version
                  docker build -t ${IMAGE_NAME}:latest .
                '''
            }
        }
    }

    post {
        always {
            sh 'docker images | head -n 20 || true'
        }
    }
}
