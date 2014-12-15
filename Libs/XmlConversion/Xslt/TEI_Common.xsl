<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xd="http://www.oxygenxml.com/ns/doc/xsl"
    xmlns:xml="http://www.w3.org/XML/1998/namespace"
    xmlns:tei="http://www.tei-c.org/ns/1.0"
    xmlns:b="#default"
    exclude-result-prefixes="xd b tei"
    version="1.0">
	<xsl:template name="InsertEndocingDesc" xmlns="http://www.tei-c.org/ns/1.0">
		<encodingDesc>
			<classDecl>
				<taxonomy xml:id="taxonomy">
					<category xml:id="taxonomy-dictionary">
						<catDesc xml:lang="cs-cz">slovník</catDesc>
						<category xml:id="taxonomy-dictionary-contemporary">
							<catDesc xml:lang="cs-cz">soudobý</catDesc>
						</category>
						<category xml:id="taxonomy-dictionary-historical">
							<catDesc xml:lang="cs-cz">dobový</catDesc>
						</category>
					</category>
					<category xml:id="taxonomy-historical_text">
						<catDesc xml:lang="cs-cz">historický text</catDesc>
						<category xml:id="taxonomy-historical_text-old_czech">
							<catDesc xml:lang="cs-cz">staročeský</catDesc>
						</category>
						<category xml:id="taxonomy-historical_text-medieval_czech">
							<catDesc xml:lang="cs-cz">středněčeský</catDesc>
						</category>
					</category>
					<category xml:id="taxonomy-scholary_text">
						<catDesc xml:lang="cs-cz">odborný text</catDesc>
					</category>
					<category xml:id="taxonomy-digitized-grammar">
						<catDesc xml:lang="cs-cz">digitalizovaná mluvnice</catDesc>
					</category>
					<category xml:id="taxonomy-card-index">
						<catDesc xml:lang="cs-cz">lístková kartotéka</catDesc>
					</category>
				</taxonomy>
			</classDecl>
		</encodingDesc>
	</xsl:template>
</xsl:stylesheet>