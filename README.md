# Sélection naturelle 3D

**Guide pratique · Windows**

Observez un petit monde où quatre factions récoltent, produisent, se reproduisent et rivalisent pour les ressources. Modifiez les règles pour comparer les effets de la pénurie, des mutations et des alliances.

[Premiers pas](#premiers-pas) · [Observer](#observer-le-monde) · [Scénarios](#choisir-un-scénario) · [Réglages](#adapter-les-règles) · [Rapports](#consulter-et-exporter-les-résultats)

## Premiers pas

1. Double-cliquez sur **[Demarrer.cmd](Demarrer.cmd)**.
2. Cliquez sur **Lancer**. Le bouton devient **Pause** ; la touche **Espace** permet aussi de suspendre ou reprendre.
3. Cliquez sur un habitant pour l'inspecter, puis consultez les onglets **Tableau de bord**, **Ressources et factions** et **Diplomatie et raids**.

Le modèle de départ comporte **4 factions de 50 habitants**, des châteaux et des mutations de vitesse, taille et perception. Les comportements spéciaux MAGA sont désactivés.

> **Pour une nouvelle partie différente**, utilisez **Nouvelle graine + relancer**. **Recommencer** rejoue avec la même graine.

Aucune installation supplémentaire n'est nécessaire sur cet ordinateur. Gardez les fichiers du jeu ensemble dans ce dossier. Une console accompagne la fenêtre et se ferme à la sortie.

## Piloter la simulation

| Commande | Effet |
|---|---|
| **Lancer / Pause** ou **Espace** | Démarrer, suspendre ou reprendre. |
| **Un pas** | Avancer d'une étape ; commencer le lendemain si le jour est terminé. |
| **Fin du jour** | Aller au bilan du jour ; avancer d'une journée si le bilan est déjà affiché. |
| **Vitesse ×1 / ×4 / ×12 / ×30** | Accélérer le calcul sans changer les règles ni le déroulement d'une expérience identique. |
| **Recommencer** | Repartir au jour 1 avec la même graine et les règles actives ou en attente, en pause. |
| **Nouvelle graine + relancer** | Choisir une autre graine et démarrer immédiatement au jour 1. Les règles en attente sont appliquées. |

### Comprendre la graine

La graine est le numéro qui initialise les tirages aléatoires. **Même version du jeu + mêmes règles + même graine = même déroulement**, déplacements compris. Elle apparaît en bas de la fenêtre.

Changer les effectifs ou les règles peut modifier la suite des événements, même avec la même graine. Pour comparer deux réglages, conservez la graine ; pour juger une tendance, essayez plusieurs graines.

## Observer le monde

### Caméra et sélection

| Geste | Action |
|---|---|
| Clic gauche + glisser | Tourner autour du terrain. |
| Molette | Zoomer ou dézoomer. |
| Clic droit + glisser | Déplacer le cadrage. |
| Double-clic gauche | Recentrer la caméra. |
| Clic sur un habitant | Afficher sa fiche et activer un suivi doux. |
| Clic sur une ressource | Suivre les quantités de ce type de ressource. |
| Clic sur le terrain vide | Quitter la sélection et le suivi. |

Le suivi laisse la caméra immobile tant que l'habitant reste près du centre. Vous pouvez continuer à tourner et zoomer. L'onglet **Caméra** propose aussi un bouton de recentrage.

### Où trouver les informations ?

| Vue | Informations utiles |
|---|---|
| **Bilan** | État du jour, population, règles en attente ; options d'affichage des animations et de la perception. |
| **Créature** | Identité, sexe, besoins, état, faction, peau, métier, traits physiques, arme et contenu du **SAC (occupé / capacité)**. |
| **Génome** | Copies de gènes héritées par l'habitant sélectionné. |
| **Tableau de bord** | Effectifs, lignées, histogrammes de vitesse, taille et perception, courbes et nuage de points. |
| **Ressources et factions** | Stocks, production, armes en réserve, habitants équipés et état des châteaux. |
| **Diplomatie et raids** | Confiance, alliances, opérations et journal des attaques. |

Dans le tableau de bord, cliquez sur une couleur ou utilisez **Phénotypes** pour filtrer les distributions. Un habitant bicolore compte comme porteur de deux lignées : leur somme peut dépasser la population. **La couleur de peau et la faction sont deux informations distinctes.**

En sélectionnant une ressource, la fiche et la courbe sous le terrain suivent sa quantité **sur la carte**. Les stocks des châteaux sont indiqués séparément ; les sacs sont exclus de cette courbe.

### Lire les personnages et les animations

- **Crête turquoise / rose** : mâle / femelle ; **grands yeux** : forte perception ; **volume du corps** : taille.
- **Peau unie ou bicolore** : lignées héritées, sans avantage biologique lié à la couleur.
- **+1 coloré** : ramassage ; **−x** : dépôt au château ; **croix rouge** : mort.
- **Cœur rose** : formation d'un couple sur le terrain. Les parents sont mis en évidence ; la naissance dépend encore de leur survie.

## Choisir un scénario

Ouvrez **Éditer les règles**, choisissez un modèle, cliquez sur **Charger le modèle**, puis **Nouvelle expérience** et **Lancer**.

| Scénario | Habitants / faction | Nourriture naturelle / jour | À observer |
|---|---:|---:|---|
| Équilibré sans mutations | 50 | 80 | Référence sans nouvelles mutations. |
| Équilibré avec mutations de vitesse | 50 | 80 | Évolution de la vitesse. |
| **Équilibré avec trois traits — défaut** | 50 | 80 | Vitesse, taille et perception. |
| Raréfaction | 50 | 80 → 20 | Baisse de 4 unités tous les deux jours. |
| Pénurie | 50 | 40 | Survie, inégalités et tensions. |
| Conflit MAGA | 50 | 80 | Faction spéciale, chef protégé, collecte accrue et raids. |
| Grand monde | 100 | 160 | Deux blocs permanents de deux factions tirés au hasard. |

**Conflit MAGA** active la faction trumpiste fictive, initialement corail : casquettes rouges, chef à crête orange et escorte. Si le chef meurt, un survivant lui succède et hérite de la crête.

**Grand monde** garde des factions initialement équivalentes, sans comportements spéciaux MAGA. La confiance est fixée à **+100 entre alliés** et **−100 entre blocs**. Pour retrouver des relations évolutives, désactivez **Diplomatie → Deux blocs permanents**.

Les modèles importables sont conservés dans [scenarios](scenarios). Des départs équivalents ne garantissent pas les mêmes résultats pour chaque graine.

## Comprendre les règles essentielles

### Survie et naissances

Chaque matin, les habitants reçoivent de l'énergie et les rations disponibles dans leur château. Ils explorent, récoltent puis doivent **rentrer vivants avec suffisamment de nourriture et d'eau**. Manger ne recharge pas leur énergie de déplacement.

Par défaut, une naissance exige la rencontre d'un **mâle et d'une femelle ayant chacun 3 nourritures**, assez d'eau et d'énergie. Les deux doivent rentrer vivants. Un couple donne au maximum un bébé par jour, né au bilan dans le château.

Le bébé hérite de copies de gènes de ses deux parents, avec brassage et mutations éventuelles. Les mutations apparaissent à la naissance ; le délai pour voir une différence dépend des générations et de la sélection. Une grande taille peut aider au combat, mais augmente aussi le coût du déplacement.

### Ressources et métiers

Les agriculteurs récoltent surtout la terre, les mineurs le minerai et le charbon, les artisans l'énergie, et les transporteurs approvisionnent les réserves, notamment en eau. Les survivants produisent le soir avec les stocks disponibles.

| Transformation | Production |
|---|---|
| 2 terre + 1 eau + 1 énergie | 3 nourritures, par les agriculteurs. |
| 1 charbon | 4 énergies, par les artisans ou transporteurs. |
| 2 minerais + 2 énergies | 1 fer, par les artisans ou transporteurs. |
| 1 fer + 2 énergies | 1 arme, par les artisans. |
| Fer + énergie, coût adapté aux effectifs | Réserve supplémentaire, par les artisans. |

L'énergie stockée sert à la production ; elle est distincte de l'énergie personnelle de déplacement. Les armes sont distribuées à l'aube suivante et renforcent toutes les factions. Les stocks ont une capacité maximale : les excédents sont perdus.

Les montagnes ralentissent les déplacements ; la nourriture dans les lacs coule. L'eau se récolte sur les berges. La nourriture naturelle et les gisements se renouvellent à l'aube selon les règles du modèle.

### Châteaux, prédation et raids

Les habitants rentrés disparaissent à l'intérieur du château et sont protégés de la prédation. Le **100 % au-dessus du château mesure ses défenses**, pas ses réserves. Les assauts les réduisent ; une réparation du soir coûte **1 fer + 2 énergies** pour restaurer **3 points**.

La prédation dépend de la force relative, notamment de la taille et des armes. Un raid attaque les défenses du château ; à zéro, les attaquants peuvent piller ses stocks. Les dégâts augmentent avec la taille des attaquants et leur équipement.

La diplomatie fonctionne **avec ou sans MAGA**, si le petit monde et les alliances sont actifs. Rencontres pacifiques et échanges améliorent la confiance ; agressions et accaparement de richesses en période de pénurie peuvent la dégrader. Par défaut, une alliance se forme à **20** de confiance et se rompt sous **5**.

Pour un raid coordonné, au moins deux factions alliées doivent partager une cible hostile (**−20 ou moins**). Elles doivent disposer de combattants nourris, hydratés et capables d'atteindre le château. Un raid organisé peut donc échouer avant l'assaut. Dans Grand monde, les relations des blocs restent fixes.

## Adapter les règles

Cliquez sur **Éditer les règles**, modifiez une valeur et validez avec **Entrée**. La description en bas explique le paramètre.

| Votre objectif | Réglage ou action |
|---|---|
| Changer les effectifs | **Habitants / faction**, de 1 à 100, puis **Appliquer et recommencer**. Les ressources de départ sont adaptées. |
| Modifier la partie en cours | **Appliquer au prochain jour** : conserver les habitants, appliquer à l'aube suivante. |
| Repartir avec les paramètres modifiés | **Nouvelle expérience**, puis **Lancer**. La graine saisie est conservée. |
| Faire un terrain allongé | **Terrain → Colonnes / Lignes** : par exemple 60 × 20, puis **Nouvelle expérience**. |
| Ajouter des catastrophes | **Monde et ressources → Tornades activées**. |
| Créer une inégalité géographique | **Monde et ressources → Répartition inégale des ressources**. |
| Conserver ou partager les réglages | **Exporter XML / Importer XML**. Le XML contient les règles, pas la sauvegarde de la population. |

**Limites du terrain :** 10 à 80 cases par côté, 3 600 cases au total. Un terrain plus grand augmente les distances sans augmenter automatiquement les ressources.

**Effectifs :** 100 est la limite du réglage initial par faction ; les naissances peuvent ensuite la dépasser. Les apports alimentaires restent fixes pendant une expérience, ce qui freine la croissance sans garantir une population stable.

> **Si l'affichage ralentit :** commencez avec 10 à 50 habitants par faction, réduisez la vitesse et désactivez **Animations des actions** et les portées de perception dans **Bilan**. Le rendu 3D utilise le processeur.

## Consulter et exporter les résultats

**Exporter CSV** enregistre les bilans des journées terminées et leurs règles : population, traits moyens, naissances et morts.

La campagne d'équilibrage comprend **3 500 simulations** : 250 essais par scénario avant et après modification, sur 40 jours.

- [Ouvrir le rapport graphique interactif](<Fine tune/equilibrage/rapport/Rapport-Equilibrage.html>) : ressources, stocks, production, armes, raids, châteaux et populations.
- [Lire le compte rendu détaillé](<Fine tune/equilibrage/rapport/COMPTE-RENDU.md>).
- [Consulter les statistiques CSV](<Fine tune/equilibrage/rapport/comparaison.csv>).

La plupart des essais conservent plusieurs factions au jour 40 : une coexistence n'est pas une victoire. Les réglages constituent un compromis testé, sans garantir des guerres ou un gagnant à chaque partie.

## Repères dans le dossier

| Emplacement | Contenu |
|---|---|
| **Demarrer.cmd** | Lanceur de Sélection naturelle 3D. |
| **scenarios/** | Sept modèles de règles XML. |
| **Fine tune/** | Outils de paramétrage, vérifications, benchmarks, aperçus et résultats. |
| **Fine tune/equilibrage/rapport/** | Rapport de la campagne et données comparatives. |
| **archive/** | Ancien Jeu de la vie de Conway : lanceur et sources. Pour le démarrer, ouvrir **archive/Demarrer-Conway.cmd**. |

L'application est écrite en **C# avec Windows Forms**, lancée par **Windows PowerShell 5.1 et .NET Framework**. Les sources nécessaires au démarrage restent à la racine.

Pour aller plus loin : [analyse du projet Primer](ANALYSE-PRIMER.md). Ce monde et sa génétique sont des règles de simulation simplifiées.
