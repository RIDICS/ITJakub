<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:p="http://www.daliboris.cz/schemata/prepisy.xsd" exclude-result-prefixes="p">
    <xsl:output method="html" omit-xml-declaration="yes" indent="yes" />
    <xsl:template match="/">
        <html>
            <head>
                <title>Přehled k přepisu památky</title>
                <style type="text/css">
                    #container { min-width: 600px; float: none; clear: both; }
                    #hlavicka {	width: 400px; float: left;}
                    #zpracovani {position: relative; display: inline-block; }
                    .popisek { font-size: smaller; font-style: italic; }
                    .exporty {background: #6699FF; text-align: center; }
                    .neexportovat {background: #FF0000; text-align: center; }
                    .puvodne {color: red; font-weight: bold; fnot-size: 85%; }
                </style>
            </head>
            <body>
              <xsl:apply-templates select="child::*" />
            </body>
        </html>
    </xsl:template>
    
    <xsl:template  match="p:Prepisy" >
      <div id="container">
        
        <div id="hlavicka">
          <table cellspacing="5" cellpadding="3" width="100%">
              <thead>
                  <tr>
                      <th>Soubor</th>
                      <th>Zpracování</th>
                      <th>Exporty</th>
                      <th>Období vzniku</th>
                      <th>Přesnější datace</th>
                      <th>Autor</th>
                      <th>Titul</th>
                      <th>Předloha</th>
                      <th>Tisk</th>
                      <th>Edice</th>
                      <th>Uložení</th>
                  </tr>
              </thead>
              
              <tbody>
              
                <xsl:apply-templates select="p:Prepis" />
              
            </tbody>
          </table>
        </div><!-- hlavicka -->
      </div><!-- container -->
      
    </xsl:template>
    
    <xsl:template match="p:Prepis">
      <tr>
          <xsl:apply-templates select="p:Soubor" />
          <xsl:apply-templates select="p:Zpracovani"/>
          <xsl:apply-templates select="p:Hlavicka"/>
  </tr>
<!--          
        <div id="zpracovani">
            <table cellspacing="5" cellpadding="3">
                <tbody>
                    <xsl:apply-templates select="p:Zpracovani"/>
                </tbody>
            </table>
            
            </div>-->
          <!-- zpracovani -->
            
     </xsl:template>
    
    <xsl:template match="p:Zpracovani">
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Fáze zpracování'" />
            <xsl:with-param name="hodnota" select="p:FazeZpracovani" />
        </xsl:call-template>
        <xsl:call-template name="Exporty" />
        
<!--        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Časové zařazení'" />
            <xsl:with-param name="hodnota" select="p:CasoveZarazeni" />
        </xsl:call-template>
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Způsob využití'" />
            <xsl:with-param name="hodnota" select="p:ZpusobVyuziti" />
        </xsl:call-template>
        <xsl:apply-templates select="p:Neexportovat"/>
        <xsl:apply-templates select="p:Exporty" />
-->    </xsl:template>
    
<!--    <xsl:template match="p:Neexportovat">
        <xsl:if test="text() = 'true'">
            <tr><td colspan="2"><b>neexportovat</b></td></tr>
        </xsl:if>
    </xsl:template>
    -->
    <xsl:template match="p:Hlavicka">
      
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Datace vzniku'" />
            <xsl:with-param name="hodnota" select="../p:ObdobiVzniku" />
        </xsl:call-template>
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Přesnější datace'" />
            <xsl:with-param name="hodnota" select="p:DataceText" />
        </xsl:call-template>
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Autor'" />
            <xsl:with-param name="hodnota" select="p:Autor" />
        </xsl:call-template>
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Titul'" />
            <xsl:with-param name="hodnota" select="p:Titul" />
        </xsl:call-template>
        <xsl:call-template name="VypisHodnotu">
        <xsl:with-param name="popisek" select="'Předloha'" />
        <xsl:with-param name="hodnota" select="p:TypPredlohyText" />
        </xsl:call-template>
        <xsl:call-template name="Tisk"/>
        <xsl:call-template name="Edice" />
        <xsl:call-template name="Ulozeni" />

      
<!--      
        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Editoři přepisu'" />
            <xsl:with-param name="hodnota" select="p:EditoriPrepisuText" />
        </xsl:call-template>
-->    </xsl:template>
    
    
    <xsl:template name="Ulozeni">
<!--        <tr>
            <xsl:call-template name="VypisJenomPopisek">
                <xsl:with-param name="text" select="'Uložení'" />
            </xsl:call-template>
-->            <td>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param  name="hodnota" select="p:ZemeUlozeni" />
                    <xsl:with-param name="posttext" select="', '"/>
                </xsl:call-template>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param name="hodnota" select="p:MestoUlozeni" />
                    <xsl:with-param name="posttext" select="', '"/>
                </xsl:call-template>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param  name="hodnota" select="p:InstituceUlozeni" />
                    <xsl:with-param name="posttext" select="'; '"/>
                </xsl:call-template>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param  name="hodnota" select="p:Signatura" />
                    <xsl:with-param name="pretext" select="'sign.: '"/>
                    <xsl:with-param name="posttext" select="'; '"/>
                </xsl:call-template>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param  name="hodnota" select="p:FoliacePaginace" />
                    <xsl:with-param name="posttext" select="'.'"/>
                </xsl:call-template>
            </td>
<!--        </tr>-->
    </xsl:template>
    
    <xsl:template name="Tisk">
<!--        <tr>
        <xsl:call-template name="VypisJenomPopisek">
            <xsl:with-param name="text" select="'Tisk'" />
        </xsl:call-template>
-->            <td>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param  name="hodnota"  select="p:Tiskar" />
                    <xsl:with-param name="posttext" select="', '"/>
                </xsl:call-template>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param  name="hodnota"  select="p:MistoTisku" />
                    <xsl:with-param name="posttext" select="'.'"/>
                </xsl:call-template>
            </td>
<!--            </tr>-->
    </xsl:template>
    
    <xsl:template name="VypisJenomPopisek">
        <xsl:param name="text"/>
        <td><span class="popisek"><xsl:value-of select="$text"/></span></td>
    </xsl:template>
    
    <xsl:template match="p:Soubor">
<!--        <tr><td><span class="popisek">Soubor</span></td>
-->            <td>
<!--        <a>-->
<!--            <xsl:attribute name="href">
                <xsl:value-of select="translate(concat('file:///', p:Adresar, '\', p:Nazev), '\', '/')"/>
            </xsl:attribute>
-->            <!-- 
            <xsl:value-of select="concat(p:Adresar, '\', p:Nazev)"/>
            -->
            <xsl:value-of select="p:Nazev"/>
<!--        </a>-->
            </td>
<!--        </tr>-->
<!--        <xsl:call-template name="VypisHodnotu">
            <xsl:with-param name="popisek" select="'Velikost'" />
            <xsl:with-param name="hodnota" select="p:Velikost" />
            <xsl:with-param name="posttext" select="' bajtů'"/>
        </xsl:call-template>
         <tr>
            <td><span class="popisek">Poslední změna</span></td>
            
            <td>
                <xsl:if test="p:Zmeneno/@prev">
                    <span class="puvodne">
                        <xsl:call-template name="FormatDate"><xsl:with-param name="DateTime" select="p:Zmeneno/@prev" /></xsl:call-template>
                    </span>
                    <xsl:text> </xsl:text>
                </xsl:if>
                <xsl:call-template name="FormatDate"><xsl:with-param name="DateTime" select="p:Zmeneno" /></xsl:call-template>
            </td>
        </tr>
-->    </xsl:template>

    <xsl:template name="Exporty">
        <td>
            <xsl:if test="p:PrvniExporty/p:Export/p:ZpusobVyuziti='Manuscriptorium'">MNS </xsl:if>
            <xsl:if test="p:PrvniExporty/p:Export/p:ZpusobVyuziti='StaroceskyKorpus'">VW </xsl:if>
        </td>
    </xsl:template>
<!--     
    <xsl:template match="p:Export">
        <tr>
            <td><span class="popisek">
                <xsl:call-template name="NahradVycetTextem"><xsl:with-param name="hodnota" select="p:ZpusobVyuziti"></xsl:with-param></xsl:call-template>
            </span></td>
            <td><xsl:call-template name="FormatDate">
                <xsl:with-param name="DateTime" select="p:CasExportu"/>
            </xsl:call-template></td>
        </tr>
        
    </xsl:template>
-->
    
    <xsl:template name="Edice">
        <xsl:choose>
            <xsl:when test="p:TitulEdice">
                <td>
                    <xsl:value-of select="concat(p:TitulEdice/text(), '; ', p:EditorEdice/text(), '. ')"/>
                    <xsl:value-of select="concat(p:MistoVydaniEdice/text(), ', ', p:RokVydaniEdice/text(), ', ' , p:StranyEdice/text(), '.')"/>
                </td>
            </xsl:when>
            <xsl:otherwise>
                <td></td>
            </xsl:otherwise>
        </xsl:choose>
    </xsl:template>
    
    <xsl:template name="VypisHodnotu">
        <xsl:param name="popisek" />
        <xsl:param name="hodnota" />
        <xsl:param name="pretext" />
        <xsl:param name="posttext" />
<!--        <tr>
            <td><span class="popisek"> <xsl:value-of select="$popisek"/></span></td>
-->            <td>
                <xsl:call-template name="VypisJenomHodnotu">
                    <xsl:with-param name="pretext" select="$pretext"/>
                    <xsl:with-param name="hodnota" select="$hodnota"/>
                    <xsl:with-param name="posttext" select="$posttext"/>
                </xsl:call-template>
            </td>
<!--        </tr>
-->    </xsl:template>
    
    <xsl:template name="VypisJenomHodnotu">
        <xsl:param name="pretext"/>
        <xsl:param name="hodnota"/>
        <xsl:param name="posttext"/>
        <xsl:if test="$hodnota and string-length($hodnota) &gt; 0">
        <xsl:if test="$pretext"><xsl:value-of select="$pretext"/></xsl:if>
        <xsl:if test="$hodnota/@prev">
            <span class="puvodne">
                <xsl:call-template name="NahradVycetTextem"><xsl:with-param name="hodnota" select="$hodnota/@prev" /></xsl:call-template>
                <xsl:text> </xsl:text>
            </span>
        </xsl:if>
            <xsl:call-template name="NahradVycetTextem"><xsl:with-param name="hodnota" select="$hodnota" /></xsl:call-template>
            <xsl:if test="$posttext"><xsl:value-of select="$posttext"/></xsl:if>
        </xsl:if>
    </xsl:template>
    <xsl:template name="VypisText">
       <xsl:param name="popisek" />
        <xsl:param name="text" />
<!--        <tr>
            <td><span class="popisek"><xsl:value-of select="$popisek"/></span></td>
-->            <td><xsl:call-template name="NahradVycetTextem"><xsl:with-param name="hodnota" select="$text" /></xsl:call-template></td>
<!--        </tr>-->
    </xsl:template>
    

    
    <xsl:template name="NahradVycetTextem">
        <xsl:param name="hodnota" />
            <xsl:choose>
                <!-- CasoveZarazeni -->
                <xsl:when test="$hodnota = 'Nezarazeno'">nezařazeno</xsl:when>
                <xsl:when test="$hodnota = 'DoRoku1800'">do r. 1800</xsl:when>
                <xsl:when test="$hodnota = 'DoRoku1500'">do r. 1500</xsl:when>
                <xsl:when test="$hodnota = 'PoRoce2000'">novočeské</xsl:when>
                <!-- ZpusobVyuziti -->
                <xsl:when test="$hodnota = 'StaroceskyKorpus'">STB</xsl:when>
                <xsl:when test="$hodnota = 'Manuscriptorium'">MNS</xsl:when>
                <xsl:when test="$hodnota = 'Manuscriptorium StredoceskyKorpus'">MNS, StřčTB</xsl:when>
                <xsl:when test="$hodnota = 'Manuscriptorium StaroceskyKorpus'">MNS, STB</xsl:when>
                <xsl:when test="$hodnota = 'StredoceskyKorpus'">StřčTB</xsl:when>
                <!--  Neexportovat -->
                <xsl:when test="$hodnota = 'true'">ano</xsl:when>
                <xsl:when test="$hodnota = 'false'">ne</xsl:when>
                
                <!-- FazeZpracovani -->
<!--                <xsl:when test="$hodnota = 'Odlozeno'">odloženo</xsl:when>
                <xsl:when test="$hodnota = 'Zadano'">zadáno</xsl:when>
                <xsl:when test="$hodnota = 'Prepsano'">přepsáno</xsl:when>
                <xsl:when test="$hodnota = 'TextovaKontrola'">textová kontrola</xsl:when>
                <xsl:when test="$hodnota = 'FormalniKontrola'">formální kontrola</xsl:when>
                <xsl:when test="$hodnota = 'OdlozitExport'">odložený export</xsl:when>
                <xsl:when test="$hodnota = 'Exportovat'">exportovat</xsl:when>
                <xsl:when test="$hodnota = 'Exportovano'">exportováno</xsl:when>
-->                    
                <xsl:when test="$hodnota = 'Odlozeno'">nespolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'Zadano'">nespolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'Prepsano'">nespolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'TextovaKontrola'">nespolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'FormalniKontrola'">nespolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'OdlozitExport'">nespolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'Exportovat'">spolehlivé</xsl:when>
                <xsl:when test="$hodnota = 'Exportovano'">spolehlivé</xsl:when>
                
                <!-- zbylé hodnoty -->
                <xsl:otherwise><xsl:value-of select="$hodnota/text()"/></xsl:otherwise>
           </xsl:choose>
    </xsl:template>
    
    <xsl:template name="FormatDate">
        <xsl:param name="DateTime" />
        
        <xsl:variable name="year">
            <xsl:value-of select="substring($DateTime,1,4)" />
        </xsl:variable>
        
        <xsl:variable name="month-temp">
            <xsl:value-of select="substring-after($DateTime,'-')" />
        </xsl:variable>
        
        <xsl:variable name="month">
            <xsl:value-of select="substring-before($month-temp,'-')" />
        </xsl:variable>
        
        <xsl:variable name="day-temp">
            <xsl:value-of select="substring-after($month-temp,'-')" />
        </xsl:variable>
        
        <xsl:variable name="day">
            <xsl:value-of select="substring($day-temp,1,2)" />
        </xsl:variable>
        
        
        
        <xsl:variable name="time">
            <xsl:value-of select="substring-after($DateTime,'T')" />
        </xsl:variable>
        
        <xsl:variable name="hh">
            <xsl:value-of select="substring($time,1,2)" />
        </xsl:variable>
        
        <xsl:variable name="mm">
            <xsl:value-of select="substring($time,4,2)" />
        </xsl:variable>
        
        <xsl:variable name="ss">
            <xsl:value-of select="substring($time,7,2)" />
        </xsl:variable>
        
        
        <xsl:value-of select="$day"/>
        <xsl:value-of select="'. '"/>
        <!--18.-->
        <xsl:value-of select="$month"/>
        <xsl:value-of select="'. '"/>
        <!--18.03.-->
        <xsl:value-of select="$year"/>
        <xsl:value-of select="' '"/>
        <!--18.03.1976 -->
            <xsl:value-of select="$hh"/>
            <xsl:value-of select="':'"/>
        <!--18.03.1976 13: -->
            <xsl:value-of select="$mm"/>
        <!--18.03.1976 13:24 -->
            <xsl:value-of select="':'"/>
            <xsl:value-of select="$ss"/>
       <!--18.03.1976 13:24:55 -->
        
    </xsl:template>
    
    
</xsl:stylesheet>
