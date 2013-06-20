<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="versions">
<html>
    <head>
        <title>Sharp3D.Math ChangeLog</title>
    </head>
    <style>
        body, input, textarea, select, option 
        {
            font-size: 85%;
        	font-family: verdana, arial, helvetica, sans-serif;
        }
        code, pre, tt 
        {
	        font-family: Courier, "Courier New", monospace;
        }
        hr 
        {
	        border: 0px;
	        color: black;
	        background-color: black;
	        height: 1px;
        }
        h1, h2, h3, h4 
        {
	        font-weight: bold;
	        color: #000066;
	        background-color: transparent;
        }
        h1 { font-size: 130%; }
        h2 { font-size: 125%; }
        h3 { font-size: 110%; }
        h4 { font-size: 100%; }

        li 
        {
	        padding-top: 3px;
	        padding-bottom: 3px;
	        list-style-type: square;
        }
    </style>
    <body>
        <h1>Sharp3D.Math ChangeLog</h1>
        <hr/>
        <xsl:apply-templates select="version">
            <xsl:sort select="@name" order="descending"/>
        </xsl:apply-templates>
    </body>
</html>
</xsl:template>

<xsl:template match="version">
    <h3>Version <xsl:value-of select="@name"/></h3>
    <b><xsl:value-of select="@date"/></b>
    <ul>
    <xsl:for-each select="add">
        <li><b>Added:</b> <xsl:apply-templates /></li>
    </xsl:for-each>
    <xsl:for-each select="fix">
        <li><b>Fixed:</b> <xsl:apply-templates /></li>
    </xsl:for-each>
    <xsl:for-each select="change">
        <li><b>Changed:</b> <xsl:apply-templates /></li>
    </xsl:for-each>
    </ul>
    <hr/>
</xsl:template>

</xsl:stylesheet>

  