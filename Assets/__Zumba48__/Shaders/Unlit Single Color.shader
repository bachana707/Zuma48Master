Shader "Unlit Single Color" {
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
		Category{
		   Lighting Off
		   ZWrite On
		   Cull Back
		   SubShader {

				Tags { "Queue" = "Background" }

				Pass {
				   SetTexture[_MainTex] {
						constantColor[_Color]
						Combine texture * constant, texture * constant
					 }
				}
			}
	}
}