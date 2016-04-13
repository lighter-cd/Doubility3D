Shader "Hidden/Next_Generation/InvisibleMask" {
   SubShader {
     // draw after all opaque objects:
     Tags { 
		"Queue"="Geometry+99" 
		"RenderType" = "Opaque"
		}
     Pass {
		ZWrite On
		ColorMask 0
     }
   } 
 }