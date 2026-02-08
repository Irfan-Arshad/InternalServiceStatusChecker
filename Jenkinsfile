pipeline {
  agent any

  environment {
    IMAGE_LOCAL  = "issc-api"
    IMAGE_REMOTE = "irfantechno/issc-api"
  }

  stages {
    stage('Checkout') {
      steps { checkout scm }
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
          docker build -t ${IMAGE_LOCAL}:latest .
          docker tag ${IMAGE_LOCAL}:latest ${IMAGE_REMOTE}:latest
        '''
      }
    }

    stage('Docker Push') {
      steps {
        withCredentials([usernamePassword(credentialsId: 'dockerhub-creds', usernameVariable: 'DH_USER', passwordVariable: 'DH_PASS')]) {
          sh '''
            echo "$DH_PASS" | docker login -u "$DH_USER" --password-stdin
            docker push ${IMAGE_REMOTE}:latest
            docker logout
          '''
        }
      }
    }
  }

  post {
    always {
      sh 'docker images | head -n 25 || true'
    }
  }
}
