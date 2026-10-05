# Analyse : Simulating Natural Selection — Primer

Vidéo identifiée : [Simulating Natural Selection](https://www.youtube.com/watch?v=0ZGbIKd0XrM), chaîne **Primer**.

L'analyse repose sur la [transcription consultée](https://rosetta.to/u/primerblobs/simulating-natural-selection) et le [moteur publié par l'auteur](https://github.com/Helpsypoo/primerpython/blob/master/blender_scripts/tools/natural_sim.py). Elle porte sur les mécanismes ; elle ne constitue pas une inspection image par image de la vidéo. Notre code et nos modèles visuels sont créés pour ce projet.

## Mécanismes de la vidéo

| Mécanisme | Règle |
|---|---|
| Journée | La nourriture apparaît sur le terrain ; les créatures quittent les abris périphériques. |
| Survie | Une ressource et un retour vivant permettent de survivre. Sans nourriture ou sans retour, l'individu meurt. |
| Reproduction | Deux ressources et un retour permettent de survivre et de produire un descendant. |
| Hérédité | Les descendants héritent des traits ; une mutation peut les augmenter ou les diminuer. Les parents ne changent pas. |
| Vitesse | Facilite l'accès aux ressources, avec un coût énergétique. |
| Taille | Un individu au moins 20 % plus grand peut manger une autre créature. |
| Perception | Permet de détecter nourriture, proies et prédateurs, donc de poursuivre ou fuir. |
| Coût | À chaque pas : `taille³ × vitesse² + perception`. |
| Environnement | La pénurie brutale ou progressive modifie la sélection et peut provoquer une extinction. |

Source : [transcription](https://rosetta.to/u/primerblobs/simulating-natural-selection).

## Détails vérifiés dans le code de Primer

Le fichier `natural_sim.py` contient un rapport de prédation de **1,2**, une probabilité de mutation de **0,05** et une variation de **±0,1**. Il définit une énergie initiale de **800**, une portée de base de **25** et une marge de retour de **2**.

Le retour devient prioritaire après deux ressources, ou après une ressource lorsque le budget restant devient trop faible pour poursuivre prudemment. Les individus à l'abri sont protégés des prédateurs. Un prédateur récupère aussi les ressources transportées par sa victime.

Le dépôt contient également des mécanismes d'altruisme, utilisés dans d'autres expériences et exclus de notre adaptation. Le fichier ne permet pas à lui seul d'identifier tous les paramètres de chaque séquence de la vidéo.

Source primaire : [code de Primer](https://github.com/Helpsypoo/primerpython/blob/master/blender_scripts/tools/natural_sim.py).

## Choix de notre adaptation

La scène est en **3D perspective** : maillages des créatures, cubes de nourriture, éclairage, ombres au sol et caméra orbitale. Les déplacements se font sur un terrain plat. Modifier la caméra ne change pas l'expérience.

- Terrain de 300 × 300 unités, 25 individus et 100 ressources initialement. Chaque matin, les individus partent de points aléatoires sur les bords.
- Directions ajustées immédiatement, errance aléatoire, rebond aux limites, fuite prioritaire devant un prédateur visible. Le code original utilise une rotation avec accélération.
- Portée = `portée de base × perception`. Le contact dépend de la taille du corps ; le code original utilise une distance fixe.
- La nourriture contribue aux seuils, sans recharger l'énergie de déplacement. Le budget est renouvelé au réveil.
- Durée maximale réglable, initialement 600 pas. Les individus dehors à l'échéance meurent. Si tous sont morts ou rentrés, le jour se termine plus tôt.
- Reproduction sexuée ajoutée à la demande de l'utilisateur : un mâle et une femelle doivent se rencontrer sur le terrain avec deux nourritures chacun, puis rentrer tous deux vivants. Un bébé par couple et par jour. Cette extension remplace la reproduction individuelle décrite dans la vidéo.
- Génome de jeu diploïde : dix gènes par trait, copies transmises par chaque parent, brassage par segments et mutations sur les copies. La contribution des marqueurs et leur dominance donnent les traits exprimés, sans moyenne directe des traits des parents. Les fondateurs possèdent une diversité réglable.
- Apparence liée au génotype : grands yeux pour une forte perception, volume pour la taille, quatre couleurs de peau correspondant aux bords de départ des fondateurs. Deux marqueurs de peau différents occupent chacun la moitié des faces du corps. Le marqueur de peau ne donne aucun avantage direct pour la sélection et reste héritable malgré les déplacements.
- Dominance équilibrée : les emplacements qui favorisent la variante haute et ceux qui favorisent la variante basse ont le même poids total. Le tableau de bord distingue les phénotypes exprimés, les porteurs de lignées et les proportions des marqueurs génétiques.
- Traits bornés à [0,2 ; 4], population plafonnée à 500. Les naissances bloquées apparaissent dans le bilan et le CSV.
- Ordre des individus mélangé à chaque pas pour limiter un avantage systématique d'ancienneté. Les interactions sont résolues dans cet ordre.

Ces choix ne garantissent pas les mêmes courbes que la vidéo.

## Éditeur de règles

**Éditer les règles** ouvre un formulaire avec descriptions et aperçu de la formule énergétique.

| Thème | Paramètres |
|---|---|
| Environnement | Nourriture, durée du jour, énergie, rythme et plancher de raréfaction |
| Survie et reproduction | Seuils alimentaires par parent, distance de rencontre, marge de retour, prédation et rapport de taille |
| Coûts et perception | Coefficients, exposants de taille et vitesse, portée |
| Mutations | Activation indépendante des trois groupes de gènes, probabilité par copie, amplitude sur un marqueur |
| Génétique | Brassage entre gènes voisins, force de dominance |
| Nouvelle expérience | Population et centres des traits initiaux, diversité génétique, proportion de femelles, graine aléatoire |

**Appliquer au prochain jour** conserve les individus et applique les paramètres à l'aube suivante. Le calendrier de raréfaction repart à cette aube. Les paramètres initiaux n'affectent les individus qu'au prochain redémarrage.

**Nouvelle expérience** repart immédiatement au jour 1 avec tous les paramètres.

Les modèles permettent d'essayer les mutations désactivées, la vitesse seule, les trois traits, la raréfaction progressive ou dix ressources. Pour tester une pénurie brutale sur une population adaptée, chargez ce dernier modèle et appliquez-le au prochain jour.

L'import/export **XML** conserve les paramètres, validés avant application. Le CSV associe chaque bilan aux règles actives. L'éditeur modifie les paramètres et les exposants ; les stratégies de déplacement restent définies dans le C#.
