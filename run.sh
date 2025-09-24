#!/bin/bash
echo "Starting Display Capture Tool..."
cd DisplayCapture
dotnet run --urls "https://localhost:5001;http://localhost:5000"