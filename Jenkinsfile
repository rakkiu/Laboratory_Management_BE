pipeline {
    agent any

    options {
        gitLabConnection('gitlab-main')
    }

    environment {
        DOTNET_CLI_TELEMETRY_OPTOUT = "1"
    }

    stages {
        stage('Checkout') {
            steps {
                gitlabCommitStatus(name: 'Checkout') {
                    echo "📦 Checking out branch: ${env.BRANCH_NAME}"
                    checkout scm
                }
            }
        }

        stage('Setup .NET SDK') {
            steps {
                gitlabCommitStatus(name: 'Setup .NET SDK') {
                    echo '🛠 Checking .NET SDK...'
                    bat '''
                        dotnet --version || (
                            echo Installing .NET SDK 8.0...
                            powershell -Command "Invoke-WebRequest https://dot.net/v1/dotnet-install.ps1 -OutFile dotnet-install.ps1"
                            powershell -File .\\dotnet-install.ps1 -Channel 8.0 -InstallDir "C:\\Program Files\\dotnet"
                        )
                        dotnet --info
                    '''
                }
            }
        }

        stage('Build Services (.NET)') {
            steps {
                gitlabCommitStatus(name: 'Build Services') {
                    echo '🏗 Building IAMService, PatientService, and ApiGateway...'
                    bat '''
                        dotnet restore OJT_BE\\src\\Services\\IAMService\\IAMService.sln
                        dotnet build OJT_BE\\src\\Services\\IAMService\\IAMService.sln -c Release --no-restore

                        dotnet restore OJT_BE\\src\\Services\\PatientService\\PatientService.sln
                        dotnet build OJT_BE\\src\\Services\\PatientService\\PatientService.sln -c Release --no-restore

                        dotnet restore OJT_BE\\src\\Gateway\\ApiGateway\\ApiGateway.csproj
                        dotnet build OJT_BE\\src\\Gateway\\ApiGateway\\ApiGateway.csproj -c Release --no-restore
                    '''
                }
            }
        }

        stage('Build Docker Images') {
            steps {
                gitlabCommitStatus(name: 'Build Docker Images') {
                    echo '🐳 Building Docker images...'
                    bat '''
                        docker build -t iamservice:latest -f OJT_BE\\src\\Services\\IAMService\\Dockerfile .
                        docker build -t patientservice:latest -f OJT_BE\\src\\Services\\PatientService\\Dockerfile .
                        docker build -t apigateway:latest -f OJT_BE\\src\\Gateway\\ApiGateway\\Dockerfile .
                    '''
                }
            }
        }

        stage('Verify Images') {
            steps {
                gitlabCommitStatus(name: 'Verify Images') {
                    echo '🔎 Listing built images...'
                    bat 'docker images'
                }
            }
        }
    }

    post {
        success {
            updateGitlabCommitStatus name: 'pipeline', state: 'success'
            echo "✅ Docker images built successfully!"
        }
        failure {
            updateGitlabCommitStatus name: 'pipeline', state: 'failed'
            echo "❌ Pipeline failed."
        }
        always {
            echo "🏁 Build finished at ${new Date()}"
        }
    }
}
