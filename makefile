all: ditherMeThis
	mcs -r:System.Windows.Forms -r:System.Data -r:System.Drawing -out:ditherMeThis ./*.cs
