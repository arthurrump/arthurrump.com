{
  inputs = {
    flake-parts.url = "github:hercules-ci/flake-parts";
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";

    pelican-plugins = {
      url = "github:getpelican/pelican-plugins";
      flake = false;
    };

    picocss = {
      url = "github:picocss/pico/v2.1.1";
      flake = false;
    };
  };

  outputs = inputs@{ self, flake-parts, ... }:
    flake-parts.lib.mkFlake { inherit inputs; } {
      systems = [ "x86_64-linux" "aarch64-linux" "aarch64-darwin" "x86_64-darwin" ];
      perSystem = { config, self', inputs', pkgs, system, ... }: 
      let 
        tools = with pkgs; with python312Packages; [
          python
          pandoc
          haskellPackages.pandoc-crossref
          pelican
          markdown
          typogrify
          beautifulsoup4
          ruamel-yaml

          typer
          jinja2

          dart-sass
        ];

        sassCmd = "sass theme/style/style.scss theme/static/css/style.css --no-source-map";

        PELICAN_PLUGINS = "${inputs.pelican-plugins}";
        SASS_PATH = "${inputs.picocss}/scss/";
      in {
        devShells.default = pkgs.mkShell {
          packages = tools ++ [
            (pkgs.writeShellScriptBin "develop" ''
              ${sassCmd} --watch & 
              pelican -l -r &&
              kill $!
            '')
            (pkgs.writeShellScriptBin "cms" ''
              python -m cms
            '')
          ];
          inherit PELICAN_PLUGINS SASS_PATH;
        };

        packages.default = pkgs.stdenv.mkDerivation {
          name = "site";
          src = ./.;
          buildInputs = tools;
          inherit PELICAN_PLUGINS SASS_PATH;
          phases = [ "unpackPhase" "buildPhase" "installPhase" ];
          buildPhase = ''
            ${sassCmd}
            pelican -s publishconf.py
          '';
          installPhase = ''
            mkdir -p $out
            cp -r ./output/* $out/
          '';
        };
      };
    };
}
