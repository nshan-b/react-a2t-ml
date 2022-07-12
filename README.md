# react-a2t-ml

## Quick Overview
This is a quick example of using a react-speech-recognition hook (a wrapper for the <a href="https://developer.mozilla.org/en-US/docs/Web/API/SpeechRecognition">Web Speech API</a>) to convert audio input to text. Sentiments are saved to a local db (you have to provide the config as an appsettings.json file).

Data is trained from a twitter dataset using tensorflow. The model is given to MLNet on the backend for inference when calling the /sentiments API (POST).
