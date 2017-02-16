FROM debian:wheezy

ENV MONO_VERSION 4.6.2.7

RUN apt-get update \
  && apt-get install -y curl \
  && rm -rf /var/lib/apt/lists/*

RUN apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF

RUN echo "deb http://download.mono-project.com/repo/debian wheezy/snapshots/$MONO_VERSION main" > /etc/apt/sources.list.d/mono-xamarin.list \
  && apt-get update \
  && apt-get install -y wget binutils mono-devel ca-certificates-mono fsharp mono-vbnc nuget referenceassemblies-pcl \
  && rm -rf /var/lib/apt/lists/* /tmp/*


RUN mkdir -p /usr/src/app/source /usr/src/app/build
WORKDIR /usr/src/app/source
COPY . /usr/src/app/source
RUN nuget update -self
RUN nuget restore -NonInteractive
RUN cp -R /usr/src/app/source/packages/grpc.dependencies.zlib.redist.1.2.8.10/build/native /usr/src/app/build/
RUN cp -R /usr/src/app/source/packages/grpc.dependencies.openssl.redist.1.0.204.1/build/native /usr/src/app/build/
RUN xbuild /property:Configuration=Release
