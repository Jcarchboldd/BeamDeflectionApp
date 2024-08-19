# Beam Deflection Calculator

## Overview

This project is a Blazor-based web application designed to calculate the deflection across a beam with 10 equally spaced nodes, fixed at both ends. The application utilizes machine learning to predict deflections for different load positions and magnitudes applied to the beam.

### Key Features:
- **Beam Setup**: The beam is divided into 10 nodes with equal lengths between each node, fixed at both ends.
- **Deflection Prediction**: The app calculates deflection at all nodes for a given load applied at a specific node.
- **ML Model**: The project includes modules for generating training data and training the model, with ongoing development on the deflection prediction module.

## Technologies Used

- **.NET 8 Core**: Framework used for the backend and Blazor web application.
- **Blazor**: Frontend framework for creating interactive user interfaces.
- **ML.NET**: Machine learning library used to train and deploy the deflection prediction model.
- **Minimal API**: Provides lightweight endpoints for API interactions.
- **Docker**: Containerization for consistent and portable deployment.
- **Training Module**: A dedicated module for generating data and training the machine learning model.
- **Prediction Module**: Under development, this module will predict deflections using the trained model.

## Project Structure

- **Training Module**: Generates simulated beam deflection data and uses ML.NET to train a model for predicting deflection at each node.
- **Prediction Module**: (In development) Will use the trained ML model to predict deflections for new load conditions.
- **API**: Exposes endpoints via a minimal API to trigger model training and (in the future) predictions.

## Getting Started

1. **Clone the Repository**:  
   ```bash
   git clone https://github.com/yourusername/beam-deflection-calculator.git
   ```
2. **Run the Application**:
   ```bash
   docker-compose up --build
   ```
3. **Access the Blazor Application**:
   Navigate to http://localhost:yourport to interact with the web interface.

