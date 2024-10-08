<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet 
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:bk="http://www.example.com/bankruptcy"
    exclude-result-prefixes="bk"
    version="1.0">

    <xsl:template match="/">
        <html>
            <head>
                <title>Job Application</title>
            </head>
            <body>
                <h1>Job Application Details</h1>
                <p><strong>Employer Name:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:name"/></p>
                <p><strong>Legal Form:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:legalForm"/></p>
                <p><strong>ICO:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:ico"/></p>
                <p><strong>DIC:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:dic"/></p>
                <h2>Address</h2>
                <p><strong>City:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:address/bk:city"/></p>
                <p><strong>Street:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:address/bk:street"/></p>
                <p><strong>ZIP Code:</strong> <xsl:value-of select="bk:jobApplication/bk:employer/bk:address/bk:zipCode"/></p>
                <h2>Contact Information</h2>
                <p><strong>Name:</strong> <xsl:value-of select="bk:jobApplication/bk:contact/bk:name"/></p>
                <p><strong>Phone:</strong> <xsl:value-of select="bk:jobApplication/bk:contact/bk:phone"/></p>
                <p><strong>Email:</strong> <xsl:value-of select="bk:jobApplication/bk:contact/bk:email"/></p>
                <h2>Attachments</h2>
                <ul>
                    <xsl:for-each select="bk:jobApplication/bk:attachments/bk:attachment">
                        <li><xsl:value-of select="."/></li>
                    </xsl:for-each>
                </ul>
                <p><strong>Insolvency Date:</strong> <xsl:value-of select="bk:jobApplication/bk:insolvencyDate"/></p>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>
