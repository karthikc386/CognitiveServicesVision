# CognitiveServicesVision 
**_#azure #ReadTextinImages #Microsoft.Azure.CognitiveServices.Vision.ComputerVision_**

## Read Text in Images
Optical character recognition (OCR) is a subset of computer vision that deals with reading text in images and documents. The Azure Computer Vision service provides two APIs for reading text

## Analyze Image with Computer Vision
Computer Vision cognitive service provides pre-built models for common computer vision tasks, including analysis of images to suggest captions and tags, detection of common objects, landmarks, celebrities, brands, and the presence of adult content. You can also use the Computer Vision service to analyze image color and formats, and to generate “smart-cropped” thumbnail images.

## How should I use these?
a) Clone the project.  <br />
b) Create a Cognitive Services resource: https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account <br />
c) In the appSettings.json file set Cognitive Services EndPoint and Key

 "CogSvc": {
    "Endpoint": "https://yoururl.cognitiveservices.azure.com/",
    "Key": "yourKey"
  }

## 1. Using the OCR API

**Input** - Image file captured from WebCam
![image](https://user-images.githubusercontent.com/7878694/143863886-82b01c60-cfec-4310-b787-2d07eeaac387.png)

**Output** - Rendered Text from Image
![image](https://user-images.githubusercontent.com/7878694/143863080-148b6f8e-4c4d-475c-be7a-c201cbae4ff5.png)

## 2. Using the Read API

**Input** - Image file captured from WebCam
![image](https://user-images.githubusercontent.com/7878694/143863934-afdcc822-1faf-4e3e-b84b-af1565bb4d4f.png)

**Output** - Rendered Text from Image
![image](https://user-images.githubusercontent.com/7878694/143863252-a6064014-514b-4e48-b74f-ee79834b8122.png)

## 1. Using the OCR API

**Input** - Image file captured from WebCam
![image](https://user-images.githubusercontent.com/7878694/143863886-82b01c60-cfec-4310-b787-2d07eeaac387.png)

**Output** - Rendered Text from Image
![image](https://user-images.githubusercontent.com/7878694/143863080-148b6f8e-4c4d-475c-be7a-c201cbae4ff5.png)
