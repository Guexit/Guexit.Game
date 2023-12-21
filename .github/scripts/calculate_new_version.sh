get_current_version() {
  git describe --tags --abbrev=0 2>/dev/null || echo "0.0.0"
}

get_part_to_increment() {
  local increment="patch"
  while read -r commit; do
    if [[ $commit =~ BREAKING[[:space:]]CHANGE ]]; then
      echo "major"
      return
    elif [[ $commit =~ ^feat ]]; then
      increment="minor"
    elif ! [[ $commit =~ ^(feat|fix|chore|docs|style|refactor|perf|test|build|ci|revert)\: ]]; then
      increment="minor"
    fi
  done <<< "$(git log $(git describe --tags --abbrev=0)..HEAD --pretty=format:%s)"
  echo $increment
}

increment_version() {
  local version=$1
  local increment=$2
  local major=$(echo $version | cut -d. -f1)
  local minor=$(echo $version | cut -d. -f2)
  local patch=$(echo $version | cut -d. -f3)

  case $increment in
    "major")
      major=$((major+1))
      minor=0
      patch=0
      ;;
    "minor")
      minor=$((minor+1))
      patch=0
      ;;
    "patch")
      patch=$((patch+1))
      ;;
  esac

  echo "$major.$minor.$patch"
}

current_version=$(get_current_version)
echo "Current version: $current_version"

increment=$(get_part_to_increment)
new_version=$(increment_version $current_version $increment)
echo "New version: $new_version"

echo "new-version=$new_version" >> $GITHUB_OUTPUT