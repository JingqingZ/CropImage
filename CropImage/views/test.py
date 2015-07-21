outfile = open("test.html", "w")
rows = 6
cols = 5
for i in range(0, rows):
	for j in range(0, cols):
		outfile.write("<img src=\"../Images/ICL3/1/" + repr(i * cols + j) + ".bmp\">\n")
outfile.close()