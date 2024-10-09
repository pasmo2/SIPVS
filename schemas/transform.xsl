<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0"
    xmlns:app="http://www.example.com/job-application"
    xmlns:xhtml="http://www.w3.org/1999/xhtml">
    
    <!-- Output HTML with indentation -->
    <xsl:output method="html" indent="yes" />

    <!-- Template for transforming the entire job application -->
    <xsl:template match="/">
        <html xmlns="http://www.w3.org/1999/xhtml">
        <head>
            <title>Žiadosť o zamestnanie</title>
        </head>
        <body>
            <h1>Detaily</h1>

            <!-- Employer Section -->
            <h2>Informácie o zamestnávateľovi</h2>
            <p><strong>Meno zamestnávateľa: </strong> <xsl:value-of select="app:jobApplication/app:employer/app:person/app:fullName"/></p>
            <p><strong>Titul: </strong> <xsl:value-of select="app:jobApplication/app:employer/app:person/app:degree"/></p>
            <p><strong>ICO: </strong> <xsl:value-of select="app:jobApplication/app:employer/@ico"/></p>
            <p><strong>DIC: </strong> <xsl:value-of select="app:jobApplication/app:employer/@dic"/></p>
            <p><strong>Legal Form: </strong> <xsl:value-of select="app:jobApplication/app:employer/@legalForm"/></p>

            <!-- Employer Address -->
            <h3><strong>Adresa zamestnávateľa: </strong></h3>
            <p><strong>Názov ulice: </strong> 
                <xsl:value-of select="app:jobApplication/app:employer/app:address/app:streetName"/>
            </p>
            <p><strong>Číšlo domu: </strong> 
                <xsl:value-of select="app:jobApplication/app:employer/app:address/app:streetNumber"/>
            </p>
            <p><strong>Obec: </strong> 
                <xsl:value-of select="app:jobApplication/app:employer/app:address/app:city"/>
            </p>
            <p><strong>Časť obce: </strong> 
                <xsl:value-of select="app:jobApplication/app:employer/app:address/app:cityDistrict"/>
            </p>
            <p><strong>PSČ: </strong> 
                <xsl:value-of select="app:jobApplication/app:employer/app:address/app:zipCode"/>
            </p>

            <!-- Employer Contact -->
            <p><strong>Telefón zamestnávateľa: </strong> <xsl:value-of select="app:jobApplication/app:employer/app:contact/app:phone"/></p>
            <p><strong>Email zamestnávateľa: </strong> <xsl:value-of select="app:jobApplication/app:employer/app:contact/app:email"/></p>

            <!-- Candidate Section -->
            <h2>Informácie o uchádzačovi</h2>
            <p><strong>Meno uchádzača: </strong> <xsl:value-of select="app:jobApplication/app:candidate/app:person/app:fullName"/></p>
            <p><strong>Titul: </strong> <xsl:value-of select="app:jobApplication/app:candidate/app:person/app:degree"/></p>

            <!-- Candidate Address -->
            <h3><strong>Adresa uchádzača: </strong></h3>
            <p><strong>Názov ulice: </strong> 
                <xsl:value-of select="app:jobApplication/app:candidate/app:address/app:streetName"/>
            </p>
            <p><strong>Číšlo domu: </strong> 
                <xsl:value-of select="app:jobApplication/app:candidate/app:address/app:streetNumber"/>
            </p>
            <p><strong>Obec: </strong> 
                <xsl:value-of select="app:jobApplication/app:candidate/app:address/app:city"/>
            </p>
            <p><strong>Časť obce: </strong> 
                <xsl:value-of select="app:jobApplication/app:candidate/app:address/app:cityDistrict"/>
            </p>
            <p><strong>PSČ: </strong> 
                <xsl:value-of select="app:jobApplication/app:candidate/app:address/app:zipCode"/>
            </p>

            <!-- Candidate Contact -->
            <p><strong>Telefón uchádzača: </strong> <xsl:value-of select="app:jobApplication/app:candidate/app:contact/app:phone"/></p>
            <p><strong>Email uchádzača: </strong> <xsl:value-of select="app:jobApplication/app:candidate/app:contact/app:email"/></p>

            <!-- Start Date -->
            <h2>Dátum začiatku práce</h2>
            <p><strong>Dátum: </strong> <xsl:value-of select="app:jobApplication/app:startDate"/></p>

            <!-- Attachments -->
            <h2>Prílohy</h2>
            <p><strong>Prílohy: </strong></p>
            <xsl:for-each select="app:jobApplication/app:attachments/app:attachment">
                <p><xsl:value-of select="."/></p>
            </xsl:for-each>
        </body>
        </html>
    </xsl:template>

</xsl:stylesheet>