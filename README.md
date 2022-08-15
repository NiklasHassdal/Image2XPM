# Image2XPM
A utility used to convert images to an X PixMap (XPM).  
  
X PixMap is an image format that contains a C definition which means that the XPM files can be included in your source code (Example: ```#include "image.xpm"```) and therefore it'll be embedded in the executable.
## Usage
The simplest way to use this utility is to simply drag an image and drop on Image2XPM.exe. In case you want to customize the output file you can use the following parameters:  
```--output, -o```  
Used to specify an output file. The default will be the same as the input file but with the .XPM file extension.  
```--variable-name, -vn```  
Used to specify a custom variable name to use in the XPM file. The default will be the input file name without the file extension.  
```--variable-type, -vt```  
Used to specify the variable type to be used in the XPM file. The default is ```static const char*```.  
```--valid-characters, -vc```  
Used to specify what characters to use to encode images in the XPM file. The default is ```static const char*```.  
```--alpha-limit, -al```  
Used to specify what opacities should be considered transparent. This is useful since XPM only support colored pixels or fully transparent pixels. The default value is 192.
